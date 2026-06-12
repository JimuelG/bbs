import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Certificate } from '../../shared/models/certificate';
import { CertificateParams } from '../../shared/models/certificateParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class CertificatesService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  createCertificate(payload: any): Observable<any> {
    return this.http.post<Certificate>(`${this.baseUrl}/certificate`, payload);
  }

  generateCertificatePdf(referenceNumber: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/certificate/${referenceNumber}/generate-pdf`, { responseType: 'blob' });
  }

  printCertificate(id: number) {
    return this.http.get(`${this.baseUrl}/certificate/${id}/generate-pdf`);
  }

  getAllCertificates(certificateParams: CertificateParams) {
    let params = new HttpParams()
      .set('pageSize', certificateParams.pageSize)
      .set('pageIndex', certificateParams.pageIndex);

    if (certificateParams.sort) {
      params = params.append('sort', certificateParams.sort);
    }

    if (certificateParams.search) {
      params = params.append('search', certificateParams.search);
    }

    return this.http.get<Pagination<Certificate>>(`${this.baseUrl}/certificate`, { params });
  }

  updateCertificate(id: number, data: any) {
    return this.http.put(`${this.baseUrl}/certificate/${id}`, data);
  }
}
