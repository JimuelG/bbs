import { Component, computed, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatCalendar, MatCalendarCellClassFunction } from '@angular/material/datepicker';
import { MatIcon } from '@angular/material/icon';
import { Announcement } from '../../../../shared/models/announcement';
import { AnnouncementsService } from '../../../../core/services/announcements.service';
import { DatePipe } from '@angular/common';
import { SnackbarService } from '../../../../core/services/snackbar.service';
import { MatDialog } from '@angular/material/dialog';
import { CreateAnnouncementComponent } from '../../../../shared/components/create-announcement/create-announcement.component';
import { environment } from '../../../../../environments/environment.development';
import { ConfirmationDialogComponent } from '../../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [
    MatCardModule,
    MatIcon,
    MatCalendar,
    DatePipe,
  ],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
})
export class CalendarComponent implements OnInit {
  private announcementService = inject(AnnouncementsService);
  private snackbarService = inject(SnackbarService);
  private dialog = inject(MatDialog);

  announcements = signal<Announcement[]>([]);
  previewing = false;
  selectedDate = signal<Date | null>(new Date());
  @ViewChild(MatCalendar) calendar!: MatCalendar<Date>;
  baseApiUrl = environment.apiUrl;

  selectedDayAnnouncements = computed(() => {
    const date = this.selectedDate();
    const list = this.announcements();
    if (!date || list.length === 0) return [];
    
    return list.filter(a =>
      new Date(a.scheduledAt).toDateString() === date.toDateString()
    );
  });

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.announcementService.getAnnouncements().subscribe({
      next: data => {
        this.announcements.set(data);
        if (this.calendar) {
          this.calendar.updateTodaysDate();
        }
      }, 
      error: err => console.error('Could not load announcements', err)
    });
  }

  dateClass: MatCalendarCellClassFunction<Date> = (cellDate, view) => {
    if (view === 'month') {
      const hasData = this.announcements().some(a => 
        new Date(a.scheduledAt).toDateString() === cellDate.toDateString()
      );
      return hasData ? 'has-announcement' : '';
    }
    return '';
  }

  onPreview(audioUrl: string, event: Event): void {
    event.stopPropagation();
    event.preventDefault();

    if (audioUrl) {
      this.previewing = true;
      const audio = new Audio(`${this.baseApiUrl}${audioUrl}`);
      audio.play();
      audio.onended = () => this.previewing = false;
    } else {
      this.snackbarService.error(`Failed to preview audio`);
      this.previewing = false;
    }
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

    deleteAnnouncement(id: number) {
      if (!id) return;

      const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
        width: '400px',
        data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete this announcement? This action cannot be undone.'
      }
      })

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.announcementService.deleteAnnouncement(id).subscribe({
            next: () => {
              this.snackbarService.success("Announcement deleted")
              this.loadAnnouncements();
            }
          })
        }
      })
      
    }
}