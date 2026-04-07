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

  generateCertificatePdf(referenceNumber: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}certificate/${referenceNumber}/generate-pdf`, { responseType: 'blob' });
  }

  printCertificate(id: number) {
    return this.http.get(`${this.baseUrl}certificate/${id}/generate-pdf`);
  }

  loadCertificates() {
    return this.http.get<Certificate[]>(`${this.baseUrl}certificate`);
  }

  updateCertificate(id: number, data: any) {
    return this.http.put(`${this.baseUrl}certificate/${id}`, data);
  }
}
