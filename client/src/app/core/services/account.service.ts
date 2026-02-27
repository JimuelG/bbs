import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { map, Observable } from 'rxjs';
import { RegisterWithOtpRequest } from '../../shared/models/RegisterWithOtpRequest';
import { ChangePassword } from '../../shared/models/changePassword';
import { Resident } from '../../shared/models/residents';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);

    return this.http.post<User>(`${this.baseUrl}login`, values, {params});
  }

  getUserInfo() {
    return this.http.get<User>(`${this.baseUrl}account/user-info`).pipe(
      map(user => {
        this.currentUser.set(user);
        console.log('User info retrieved:', user);
        return user;
      })
    )
  }

  register(payload: RegisterWithOtpRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}account/register`, payload);
  }

  uploadIdCard(file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.baseUrl}account/upload-id-card`, formData);
  }

  verifyResident(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}account/verify/${id}`, {});
  }

  getVerifiedResident() {
    return this.http.get<Resident[]>(`${this.baseUrl}account/residents/verified`)
  }

  changePassword(userId: string, payload: ChangePassword) {
    return this.http.patch(`${this.baseUrl}account/change-password/${userId}`, payload);
  }

  logout() {
    return this.http.post(`${this.baseUrl}account/logout`, {})
  }

  getAuthState() {
    return this.http.get<{isAuthenticated: boolean}>(`${this.baseUrl}account/auth-status`);
  }

  getAllResidents() {
    return this.http.get<Resident[]>(`${this.baseUrl}account/residents`);
  }
}
