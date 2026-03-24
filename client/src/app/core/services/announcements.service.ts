import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Announcement } from '../../shared/models/announcement';
import { AnnouncementParams } from '../../shared/models/announcementParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class AnnouncementsService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  createAnnouncement(payload: any): Observable<any> {
    return this.http.post<Announcement>(`${this.baseUrl}announcement`, payload);
  }

  previewAnnouncement(payload: any): Observable<{audioUrl: string}> {
    return this.http.post<{audioUrl: string}>(`${this.baseUrl}announcement/preview`, payload);
  }

  getAllAnnouncements(announcementParams: AnnouncementParams) {
    let params = new HttpParams()
      .set('pageSize', announcementParams.pageSize)
      .set('pageIndex', announcementParams.pageIndex);
    
    if (announcementParams.sort) {
      params = params.append('sort', announcementParams.sort);
    }

    if (announcementParams.search) {
      params = params.append('search', announcementParams.search);
    }

    return this.http.get<Pagination<Announcement>>(`${this.baseUrl}announcement`, { params });
  }

  deleteAnnouncement(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}announcement/${id}`);
  }

  triggerManual(id: number): Observable<any> {
    return this.http.post(`${this.baseUrl}announcement/trigger/${id}`, {});
  }
}
