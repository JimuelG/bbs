import { Component, inject, OnInit, resource } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { User } from '../../shared/models/user';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from '../../shared/components/change-password/change-password.component';
import { environment } from '../../../environments/environment.development';
import { CommonModule, DatePipe, UpperCasePipe } from '@angular/common';
import { ResidentRecord } from '../../shared/models/residentRecord';
import { ResidentService } from '../../core/services/resident.service';
import { SnackbarService } from '../../core/services/snackbar.service';
import { IdPreviewModalComponent } from '../../shared/components/id-preview-modal/id-preview-modal.component';

@Component({
  selector: 'app-my-profile',
  imports: [
    UpperCasePipe,
    DatePipe,
    CommonModule
  ],
  templateUrl: './my-profile.component.html',
  styleUrl: './my-profile.component.scss',
})
export class MyProfileComponent implements OnInit {
  private accountService = inject(AccountService);
  private residentService = inject(ResidentService);
  private snackbarService = inject(SnackbarService);
  private dialog = inject(MatDialog);
  user: User | null = null;
  baseApiUrl = environment.apiUrl;

  residentRecords: ResidentRecord[] = [];

  ngOnInit(): void {
    this.loadUserProfile();
    this.loadResidentRecords();
  }

  loadUserProfile() {
    this.user = this.accountService.currentUser();
  }

  loadResidentRecords(): void {
    this.residentService.getMyResidentRecords().subscribe({
      next: (records) => {
        this.residentRecords = records;
      },
      error: () => {
        this.snackbarService.error('Failed to load resident records');
      }
    })
  }

  viewSubmittedId(): void {
    if (!this.user?.idUrl || this.user.idUrl === 'N/A') {
      this.snackbarService.error('No submitted ID found');
      return;
    }

    this.dialog.open(IdPreviewModalComponent, {
      width: '900px',
      maxWidth: '90vw',
      data: {
        idUrl: this.user.idUrl,
        name: `${this.user.firstName} ${this.user.lastName}`,
        isIdVerified: this.user.isIdVerified
      }
    });
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
