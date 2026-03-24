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

  onPreview(audioUrl: string): void {
  
    if (audioUrl) {
      this.previewing = true;
      const audio = new Audio(`https://localhost:5001${audioUrl}`);
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
    this.announcementService.getAllAnnouncements(this.announcementParams).subscribe({
      next: (response) => {
        this.announcementPagination = response;
        this.announcements = response.data;
        this.totalCount = response.count;
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
    this.announcementParams.pageIndex = 1;
    this.loadAnnouncements();
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }

  onTriggerRPi(id: number)  {
    this.announcementService.triggerManual(id).subscribe({
      next: () => {
        this.snackbarService.success('Broadcast signal sent! RPi is responding...');
        this.loadAnnouncements();
      },
      error: (err) => {
        this.snackbarService.error('Failed to communicate with the broadcast system');
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
}
