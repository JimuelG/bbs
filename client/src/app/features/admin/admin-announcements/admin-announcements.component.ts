import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CreateAnnouncementComponent } from '../../../shared/components/create-announcement/create-announcement.component';
import { MatIcon } from '@angular/material/icon';
import { AnnouncementsService } from '../../../core/services/announcements.service';
import { Announcement } from '../../../shared/models/announcement';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-announcements',
  imports: [
    MatIcon,
    DatePipe
  ],
  templateUrl: './admin-announcements.component.html',
  styleUrl: './admin-announcements.component.scss',
})
export class AdminAnnouncementsComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private announcementService = inject(AnnouncementsService);

  announcements: Announcement[] = [];
  previewing = false;

  onPreview(audioUrl: string): void {
  
    if (audioUrl) {
      this.previewing = true;
      const audio = new Audio(`http://localhost:5001${audioUrl}`);
      audio.play();
      audio.onended = () => this.previewing = false;
    } else {
      this.snackbarService.error(`Failed to preview audio`);
      this.previewing = false;
    }
  }

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.announcementService.getAllAnnouncements().subscribe({
      next: (data) => {
        this.announcements = data
      }
    })
  }

  openAnnouncementModal(): void {
    const dialogRef = this.dialog.open(CreateAnnouncementComponent, {
      width: 'auto',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadAnnouncements();
    });
  }
}
