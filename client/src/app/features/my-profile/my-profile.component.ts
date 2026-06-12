import { Component, inject, OnInit, resource } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { User } from '../../shared/models/user';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from '../../shared/components/change-password/change-password.component';
import { environment } from '../../../environments/environment.development';
import { UpperCasePipe } from '@angular/common';

@Component({
  selector: 'app-my-profile',
  imports: [
    UpperCasePipe
  ],
  templateUrl: './my-profile.component.html',
  styleUrl: './my-profile.component.scss',
})
export class MyProfileComponent implements OnInit {
  private accountService = inject(AccountService);
  private dialog = inject(MatDialog);
  user: User | null = null;
  baseApiUrl = environment.apiUrl;

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.user = this.accountService.currentUser();
  }


  openChangePasswordDialog(): void {
    const dialogRef = this.dialog.open(ChangePasswordComponent, {
      width: 'auto',
      maxWidth: '80vw',
      data: this.user
    })

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.accountService.logout();
        this.loadUserProfile();
      }
    })
  }
}
