import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AnnouncementsService } from '../../../core/services/announcements.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CommonModule } from '@angular/common';
import { DialogModule } from '@angular/cdk/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-create-announcement',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    MatIcon
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
  loading = false;
  previewing = false;


  constructor() {
    this.announcementForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      message: ['', [Validators.required, Validators.maxLength(500)]],
      languageCode: ['fil-PH'],
      scheduledAt: [new Date().toISOString().slice(0, 16), Validators.required],
      isEmergency: [false]
    });
  }

  onPreview(): void {
    if (this.announcementForm.get('message')?.valid) {
      this.previewing = true;

      const payload = {
        message: this.announcementForm.value.message,
        languageCode: this.announcementForm.value.languageCode,
        isEmergency: this.announcementForm.value.isEmergency
      }

      this.announcementService.previewAnnouncement(payload).subscribe({
        next: (res) => {
          const audio = new Audio(`http://localhost:5001${res.audioUrl}`);
          audio.play();
          audio.onended = () => this.previewing = false;
        },
        error: () => {
          this.snackbarService.error('Failed to generate preview');
          this.previewing = false;
        }
      });
    }
  }

  onSubmit(): void {
    if (this.announcementForm.valid) {
      this.loading = true;
      this.announcementService.createAnnouncement(this.announcementForm.value).subscribe({
        next: (announcement) => {
          this.snackbarService.success('Announcement succesfully created');
          this.dialogRef.close(announcement);
        },
        error: (err) => {
          this.snackbarService.error(`Failed to create annoucement. ${err}`)
          this.loading = false;
        }
      })
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
