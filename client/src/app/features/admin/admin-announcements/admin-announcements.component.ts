import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CreateAnnouncementComponent } from '../../../shared/components/create-announcement/create-announcement.component';

@Component({
  selector: 'app-admin-announcements',
  imports: [],
  templateUrl: './admin-announcements.component.html',
  styleUrl: './admin-announcements.component.scss',
})
export class AdminAnnouncementsComponent {
  private dialog = inject(MatDialog);

  openAnnouncementModal(): void {
    const dialogRef = this.dialog.open(CreateAnnouncementComponent, {
      width: '450px',
      panelClass: 'custom-dialog-container'
    });
  }
}
