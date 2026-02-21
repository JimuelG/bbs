import { Component, computed, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatCalendar, MatCalendarCellClassFunction } from '@angular/material/datepicker';
import { MatIcon } from '@angular/material/icon';
import { Announcement } from '../../../../shared/models/announcement';
import { AnnouncementsService } from '../../../../core/services/announcements.service';
import { DatePipe } from '@angular/common';
import { SnackbarService } from '../../../../core/services/snackbar.service';

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

  announcements = signal<Announcement[]>([]);
  previewing = false;
  selectedDate = signal<Date | null>(new Date());
  @ViewChild(MatCalendar) calendar!: MatCalendar<Date>;

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
    this.announcementService.getAllAnnouncements().subscribe({
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
      const audio = new Audio(`http://localhost:5001${audioUrl}`);
      audio.play();
      audio.onended = () => this.previewing = false;
    } else {
      this.snackbarService.error(`Failed to preview audio`);
      this.previewing = false;
    }
  }
}