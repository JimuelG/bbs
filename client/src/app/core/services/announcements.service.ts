import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Announcement } from '../../shared/models/announcement';

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

  getAllAnnouncements() {
    return this.http.get<Announcement[]>(`${this.baseUrl}announcement`);
  }

  deleteAnnouncement(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}announcement/${id}`);
  }
}
