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

  createStaff(dto: any) {
    return this.http.post(`${this.baseUrl}staff`, dto);
  }

  updateStaff(id: number, dto: any) {
    return this.http.put(`${this.baseUrl}staff/${id}`, dto);
  }

  deleteStaff(id: number) {
    return this.http.delete(`${this.baseUrl}staff/${id}`);
  }

  
}
