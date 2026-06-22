import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { ResidentRecord } from '../../shared/models/residentRecord';

@Injectable({
  providedIn: 'root',
})
export class ResidentService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  getMyResidentRecords() {
    return this.http.get<ResidentRecord[]>(`${this.baseUrl}/account/me/records`);
  }
}
