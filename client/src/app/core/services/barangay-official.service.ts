import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BarangayOfficial } from '../../shared/models/barangayOfficial';
import { BarangayOfficialParams } from '../../shared/models/barangayOfficialParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class BarangayOfficialService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  getBarangayOfficials() {
    return this.http.get<BarangayOfficial[]>(`${this.baseUrl}/barangayofficial`);
  }

  getAllOfficials(officialParams: BarangayOfficialParams) {
    let params = new HttpParams()
      .set('pageSize', officialParams.pageSize)
      .set('pageIndex', officialParams.pageIndex)

    if (officialParams.sort) {
      params = params.append('sort', officialParams.sort);
    }

    if (officialParams.search) {
      params = params.append('search', officialParams.search);
    }

    return this.http.get<Pagination<BarangayOfficial>>(`${this.baseUrl}/barangayofficial`, { params });
  }

  getOfficialById(id: number) {
    return this.http.get<BarangayOfficial>(`${this.baseUrl}/barangayofficial/${id}`);
  }

  uploadImage(id: number, type: 'officials' | 'signature', file: File) {
    const formData = new FormData();
    formData.append('file', file);

    const uploadType = type === 'signature' ? 'signature' : 'officials';

    return this.http.post<BarangayOfficial>(
      `${this.baseUrl}/barangayofficial/${id}/upload-image?type=${uploadType}`, formData);
  }

  createOfficial(official: any) {
    return this.http.post<BarangayOfficial>(`${this.baseUrl}/barangayofficial`, official);
  }

  updateOfficial(id: number, official: any) {
    return this.http.put<void>(`${this.baseUrl}/barangayofficial/${id}`, official);
  }

  deleteOfficial(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/barangayofficial/${id}`);
  } 
}
