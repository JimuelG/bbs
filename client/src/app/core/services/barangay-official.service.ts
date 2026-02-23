import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { BarangayOfficial } from '../../shared/models/barangayOfficial';

@Injectable({
  providedIn: 'root',
})
export class BarangayOfficialService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient);

  getBarangayOfficials() {
    return this.http.get<BarangayOfficial[]>(`${this.baseUrl}barangayofficial`);
  }

  uploadImage(id: number, type: 'officials' | 'signature', file: File) {
    const formData = new FormData();
    formData.append('file', file);

    const uploadType = type === 'signature' ? 'signature' : 'officials';

    return this.http.post<BarangayOfficial>(
      `${this.baseUrl}barangayofficial/${id}/upload-image?type=${uploadType}`, formData);
  }

  createOfficial(official: any) {
    return this.http.post<BarangayOfficial>(`${this.baseUrl}barangayofficial`, official);
  }

  updateOfficial(id: number, official: any) {
    return this.http.put<void>(`${this.baseUrl}barangayofficial/${id}`, official);
  }

  deleteOfficial(id: number) {
    return this.http.delete<void>(`${this.baseUrl}barangayofficial/${id}`);
  } 
}
