import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AnnouncementsService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  createAnnouncement(payload: any): Observable<any> {
    return this.http.post(`${this.baseUrl}announcement`, {payload});
  }
}
