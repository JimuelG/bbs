import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatCalendar, MatCalendarCellClassFunction } from '@angular/material/datepicker';
import { MatIcon } from '@angular/material/icon';
import { Announcement } from '../../../../shared/models/announcement';
import { AnnouncementsService } from '../../../../core/services/announcements.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [
    MatCardModule,
    MatIcon,
    MatCalendar,
    DatePipe
  ],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss',
})
export class CalendarComponent implements OnInit {
  private announcementService = inject(AnnouncementsService);
  
  // 1. Changed to signal for reactive updates
  announcements = signal<Announcement[]>([]);

  selectedDate = signal<Date | null>(new Date());

  // 2. Computed signal will now reactively update when either 
  // announcements() or selectedDate() changes
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
      next: data => this.announcements.set(data),
      error: err => console.error('Could not load announcements', err)
    });
  }

  // 3. Updated to access the signal correctly with ()
  dateClass: MatCalendarCellClassFunction<Date> = (cellDate, view) => {
    if (view === 'month') {
      const hasData = this.announcements().some(a => 
        new Date(a.scheduledAt).toDateString() === cellDate.toDateString()
      );
      return hasData ? 'has-announcement' : '';
    }
    return '';
  }
}