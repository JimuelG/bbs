import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Certificate } from '../../shared/models/certificate';

@Injectable({
  providedIn: 'root',
})
export class CertificatesService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  createCertificate(payload: any): Observable<any> {
    return this.http.post<Certificate>(`${this.baseUrl}certificate`, payload);
  }

  loadCertificates() {
    return this.http.get<Certificate[]>(`${this.baseUrl}certificate`);
  }
}
