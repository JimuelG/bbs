import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { Observable } from 'rxjs';
import { RegisterWithOtpRequest } from '../../shared/models/RegisterWithOtpRequest';
import { ChangePassword } from '../../shared/models/changePassword';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(payload: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);

    return this.http.post<User>(`${this.baseUrl}login`, payload, {params});
  }

  register(payload: RegisterWithOtpRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}account/register`, payload);
  }

  verifyUser(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}account/verify/${id}`, {});
  }

  changePassword(userId: string, payload: ChangePassword) {
    return this.http.patch(`${this.baseUrl}account/change-password${userId}`, payload);
  }

  logout() {
    return this.http.post(`${this.baseUrl}account/logout`, {})
  }

  getAuthState() {
    return this.http.get<{isAuthenticated: boolean}>(`${this.baseUrl}account/auth-status`);
  }
}
