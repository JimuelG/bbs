import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CreateAnnouncementComponent } from '../../../shared/components/create-announcement/create-announcement.component';
import { MatIcon } from '@angular/material/icon';
import { AnnouncementsService } from '../../../core/services/announcements.service';
import { Announcement } from '../../../shared/models/announcement';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from "@angular/router";
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { Pagination } from '../../../shared/models/pagination';
import { AnnouncementParams } from '../../../shared/models/announcementParams';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment.development';
import { AnnouncementStats } from '../../../shared/models/announcementStats';

@Component({
  selector: 'app-admin-announcements',
  imports: [
    MatIcon,
    DatePipe,
    RouterLink,
    CommonModule,
    FormsModule
],
  templateUrl: './admin-announcements.component.html',
  styleUrl: './admin-announcements.component.scss',
})
export class AdminAnnouncementsComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private announcementService = inject(AnnouncementsService);

  announcements: Announcement[] = [];
  announcementPagination?: Pagination<Announcement>;
  announcementParams = new AnnouncementParams();
  totalCount = 0;
  pageSizeOPtions = [10,20,30];
  previewing = false;
  rpiStatus?: { isOnline: boolean, lastSeen: Date};
  baseApiUrl = environment.apiUrl;
  stats?: AnnouncementStats;
  broadcastingAnnouncementId: number | null = null;
  stoppingRpi = false;

  private apiOrigin = environment.apiUrl.replace(/\/api\/?$/, '');

  private getPublicFileUrl(path: string): string {
    if (!path) return '';

    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path.replace('/api/api/', '/api/');
    }

    let normalizedPath = path.startsWith('/') ? path : `/${path}`;

    normalizedPath = normalizedPath.replace('/api/api/', '/api/');

    if (!normalizedPath.startsWith('/api/')) {
      normalizedPath = `/api${normalizedPath}`;
    }

    return `${this.apiOrigin}${normalizedPath}`.replace('/api/api/', '/api/');
  }

  playingAudioUrl: string | null = null;
  currentAudio: HTMLAudioElement | null = null;

  onPreview(audioUrl: string): void {
    if (!audioUrl) {
      this.snackbarService.error('Failed to preview audio');
      this.previewing = false;
      return;
    }

    const fullUrl = this.getPublicFileUrl(audioUrl);

    if (this.currentAudio) {
      this.currentAudio.pause();
      this.currentAudio.currentTime = 0;

      if (this.playingAudioUrl === audioUrl) {
        this.previewing = false;
        this.currentAudio = null;
        this.playingAudioUrl = null;
        return;
      }
    }

    this.previewing = true;
    this.playingAudioUrl = audioUrl;
    this.currentAudio = new Audio(fullUrl);
    this.currentAudio.play();

    this.currentAudio.onended = () => {
      this.previewing = false;
      this.currentAudio = null;
      this.playingAudioUrl = null;
    }
  }

  ngOnInit(): void {
    this.loadAnnouncements();
    this.loadStats();
    this.checkStatus();
  }

  loadAnnouncements() {
    this.announcementService.getAllAnnouncements(this.announcementParams).subscribe({
      next: (response) => {
        this.announcementPagination = response;
        this.announcements = response.data;
        this.totalCount = response.count;
      }
    })
  }

  loadStats() {
    this.announcementService.getStats().subscribe({
      next: (stats) => {
        this.stats = stats;
      },
      error: () => {
        this.snackbarService.error('Failed to load announcement statistics');
      }
    })
  }

  onPageChange(event: any) {
    this.announcementParams.pageIndex = event.pageIndex + 1;
    this.announcementParams.pageSize = event.pageSize;
    this.loadAnnouncements();
  }

  updatePage(newPageIndex: number) {
    this.announcementParams.pageIndex = newPageIndex;
    this.loadAnnouncements();
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.announcementParams.pageSize = parseInt(select.value);
    this.loadAnnouncements();
  }

  onSortChange() {
    this.announcementParams.pageIndex = 1;
    this.loadAnnouncements();
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }

  onSearchChange() {
    this.announcementParams.pageIndex = 1;
    this.loadAnnouncements();
  }

  onTriggerRPi(id: number)  {
    this.announcementService.triggerManual(id).subscribe({
      next: () => {
        this.broadcastingAnnouncementId = id;
        this.snackbarService.success('Broadcast signal sent! RPi is responding...');
        this.loadAnnouncements();
      },
      error: (err) => {
        this.snackbarService.error('Failed to communicate with the broadcast system');
      }
    })
  }

  onStopRPi() {
    this.stoppingRpi = true;

    this.announcementService.stopRpiAudio().subscribe({
      next: () => {
        this.broadcastingAnnouncementId = null;
        this.stoppingRpi = false;
        this.snackbarService.success('Broadcast stopped.');
        this.loadAnnouncements();
      },
      error: () => {
        this.stoppingRpi = false;
        this.snackbarService.error('Failed to stop the broadcast system');
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
      if (result) {
        this.loadAnnouncements();
        this.loadStats();
        this.checkStatus();
      } 
    });
  }

  onDelete(id: number): void {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete this announcement? This action cannot be undone.',
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.announcementService.deleteAnnouncement(id).subscribe({
          next: () => {
            this.loadAnnouncements();
            this.snackbarService.success('Announcement deleted successfully');
          },
          error: (error) => {
            this.snackbarService.error(`Failed to delete announcement: ${error.message || error}`);
          }
        });
      }
    });
  }

  checkStatus() {
    this.announcementService.getRPiStatus().subscribe({
      next: (status) => {
        this.rpiStatus = status;
      },
      error: (err) => console.error('Could not fetch RPi status', err)
    })
  }

  onSendSms(announcement: Announcement) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'Confirm SMS Broadcast',
        message: 'Are you sure you want to send this announcement via SMS to all verified residents?',
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.announcementService.sendSmsBroadcast(announcement).subscribe({
          next: (response: any) => {
            this.snackbarService.success(response.message || 'SMS broadvast send successfully!');
          },
          error: (err) => {
            this.snackbarService.error('Failed to send SMS broadcast.');
          }
        })
      }
    })
  }
}
