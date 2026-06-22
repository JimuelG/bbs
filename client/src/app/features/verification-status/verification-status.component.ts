import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { MatIcon } from '@angular/material/icon';
import { Router } from '@angular/router';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { SnackbarService } from '../../core/services/snackbar.service';

@Component({
  selector: 'app-verification-status',
  imports: [
    MatIcon
  ],
  templateUrl: './verification-status.component.html',
  styleUrl: './verification-status.component.scss',
})
export class VerificationStatusComponent implements OnInit, OnDestroy {
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);

  private hubConnection: signalR.HubConnection | undefined;
  
  ngOnInit(){
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}/verification`, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('Connected to Verification Hub!'))
      .catch(err => console.error('Error connecting to Hub:', err));

    this.hubConnection.on('AccountVerified', async () => {
      
      this.accountService.getUserInfo().subscribe({
        next: (user) => {
          this.snackbarService.success('Account verified by admin! Redirecting...');
          this.router.navigateByUrl('/home');
        },
        error: (err) => {
          this.snackbarService.error(`Error refreshing user stats: ${err}`)
        } 
      })

    });
  }

  ngOnDestroy(){
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/account/login');
      }
    })
  }
}
