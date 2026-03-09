import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Staff } from '../../shared/models/staff';

@Injectable({
  providedIn: 'root',
})
export class StaffService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  getStaffs() {
    return this.http.get<Staff[]>(`${this.baseUrl}staff`);
  }

  
}
