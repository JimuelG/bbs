import { Component, inject, OnInit, resource } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { User } from '../../shared/models/user';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from '../../shared/components/change-password/change-password.component';

@Component({
  selector: 'app-my-profile',
  imports: [],
  templateUrl: './my-profile.component.html',
  styleUrl: './my-profile.component.scss',
})
export class MyProfileComponent implements OnInit {
  private accountService = inject(AccountService);
  private dialog = inject(MatDialog);
  user: User | null = null;
  baseUrl = 'https://localhost:5001';

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.user = this.accountService.currentUser();
  }


  openChangePasswordDialog(): void {
    const dialogRef = this.dialog.open(ChangePasswordComponent, {
      width: 'auto',
      maxWidth: '80vw'
    })

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.loadUserProfile();
      }
    })
  }
}
