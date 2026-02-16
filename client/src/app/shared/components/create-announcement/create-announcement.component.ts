import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AnnouncementsService } from '../../../core/services/announcements.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CommonModule } from '@angular/common';
import { DialogModule } from '@angular/cdk/dialog';

@Component({
  selector: 'app-create-announcement',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule
  ],
  templateUrl: './create-announcement.component.html',
  styleUrl: './create-announcement.component.scss',
})
export class CreateAnnouncementComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef);
  private snackbarService = inject(SnackbarService);
  private announcementService = inject(AnnouncementsService);

  announcementForm: FormGroup;

  constructor() {
    this.announcementForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      message: ['', [Validators.required, Validators.maxLength(500)]],
      languageCode: ['fil-PH'],
      scheduledAt: [new Date().toISOString().slice(0, 16), Validators.required],
      isEmergency: [false]
    });
  }

  onSubmit(): void {
    if (this.announcementForm.valid) {
      this.announcementService.createAnnouncement(this.announcementForm.value).subscribe({
        next: (announcement) => {
          this.snackbarService.success('Announcement succesfully created');
          this.dialogRef.close(announcement);
        },
        error: (err) => this.snackbarService.error(`Failed to create annoucement. ${err}`)
      })
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
