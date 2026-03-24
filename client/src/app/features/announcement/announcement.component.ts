import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { AnnouncementsService } from '../../core/services/announcements.service';
import { Announcement } from '../../shared/models/announcement';
import { DatePipe } from '@angular/common';
import { Pagination } from '../../shared/models/pagination';
import { AnnouncementParams } from '../../shared/models/announcementParams';

@Component({
  selector: 'app-announcement',
  standalone: true,
  imports: [
    MatIcon,
    DatePipe
  ],
  templateUrl: './announcement.component.html',
  styleUrl: './announcement.component.scss',
})
export class AnnouncementComponent implements OnInit{
  private announcementService = inject(AnnouncementsService);

  announcements: Announcement[] = [];
  announcementPagination?: Pagination<Announcement>;
  announcementParams = new AnnouncementParams();
  totalCount = 0;
  pageSizeOPtions = [10,20,30];

  ngOnInit(): void {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.announcementService.getAllAnnouncements(this.announcementParams).subscribe({
      next: (response) => {
        console.log('API RESPONSE:', response);
        this.announcements = [...this.announcements, ...response.data];
        this.totalCount = response.count;
      }
    })
  }

  loadMore() {
    this.announcementParams.pageIndex++;
    this.loadAnnouncements();
  }

  onSearch(event: any) {
    this.announcementParams.search = event.target.value;
    this.announcementParams.pageIndex = 1;
    this.announcements = [];
    this.loadAnnouncements();
  }

  resetFilters() {
    this.announcementParams = new AnnouncementParams();
    this.announcements = [];
    this.loadAnnouncements();
  }

  hasMore(): boolean {
    return this.announcements.length < this.totalCount;
  }
}
