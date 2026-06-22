import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../../shared/models/user';
import { map, Observable } from 'rxjs';
import { RegisterWithOtpRequest } from '../../shared/models/RegisterWithOtpRequest';
import { ChangePassword } from '../../shared/models/changePassword';
import { Resident } from '../../shared/models/residents';
import { RegisterAccount } from '../../shared/models/registerAccount';
import { ResetPassword } from '../../shared/models/resetPassword';
import { ResidentRecord } from '../../shared/models/residentRecord';
import { ResidentParams } from '../../shared/models/residentParams';
import { Pagination } from '../../shared/models/pagination';

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

    return this.http.post<User>(`${this.baseUrl}/login`, values, {params});
  }

  getUserInfo() {
    return this.http.get<User>(`${this.baseUrl}/account/user-info`).pipe(
      map(user => {
        this.currentUser.set(user);
        return user;
      })
    )
  }

  register(payload: RegisterAccount): Observable<any> {
    return this.http.post(`${this.baseUrl}/account/register`, payload);
  }

  checkEmailExists(email: string) {
    return this.http.get<boolean>(`${this.baseUrl}/account/email-exists`, { params: { email } });
  }

  uploadIdCard(email: string,file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.baseUrl}/account/upload-id-card/${email}`, formData);
  }

  uploadProfilePicture(email: string,file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.baseUrl}/account/upload-profile-picture/${email}`, formData);
  }

  updateResident(userId: string, residentData: any) {
    return this.http.put(`${this.baseUrl}/account/update-resident/${userId}`, residentData);
  }

  verifyResident(id: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/account/verify/${id}`, {});
  }

  getResidentRecords(id: string) {
    return this.http.get<ResidentRecord[]>(`${this.baseUrl}/account/residents/${id}/records`);
  }

  getResidentDetails(id: string) {
    return this.http.get<Resident>(`${this.baseUrl}/account/resident-details/${id}`);
  }

  sendForgotPasswordOtp(email: string) {
    return this.http.post(`${this.baseUrl}/account/send-otp`, { email });
  }

  resetPassword(resetPassword: ResetPassword) {
    return this.http.patch(`${this.baseUrl}/account/reset-password`, resetPassword)
  }

  changePassword(userId: string, payload: ChangePassword) {
    return this.http.patch(`${this.baseUrl}/account/change-password/${userId}`, payload);
  }

  logout() {
    return this.http.post(`${this.baseUrl}/account/logout`, {})
  }

  getAuthState() {
    return this.http.get<{isAuthenticated: boolean}>(`${this.baseUrl}/account/auth-status`);
  }

  getAllResidents(residentParams: ResidentParams) {
    let params = new HttpParams()
      .set('pageSize', residentParams.pageSize)
      .set('pageIndex', residentParams.pageIndex);

    if (residentParams.sort) {
      params = params.append('sort', residentParams.sort);
    }

    if (residentParams.isIdVerified !== null) {
      params = params.append('isIdVerified', residentParams.isIdVerified);
    }

    if (residentParams.search) {
      params = params.append('search', residentParams.search);
    }

    return this.http.get<Pagination<Resident>>(`${this.baseUrl}/account/residents`, { params } );
  }

  adminLogin(credentials: {email: string; password: string;}) {
    return this.http.post(`${this.baseUrl}/login?useCookies=true`, credentials, {
      withCredentials: true
    })
  }
}
