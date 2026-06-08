import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Concern } from '../../shared/models/concern';

@Injectable({
  providedIn: 'root',
})
export class ConcernService {
  private http = inject(HttpClient)
  private baseUrl = environment.apiUrl;

  createConcern(dto: any) {
    return this.http.post<{ message: string, id: number }>(`${this.baseUrl}/concerns`, dto);
  }

  uploadPhoto(concernId: number, file: File) {
    const formData = new FormData();
    formData.append('photo', file);
    return this.http.post(`${this.baseUrl}/concerns/${concernId}/upload-photo`, formData);
  }

  getConcerns() {
    return this.http.get<Concern[]>(`${this.baseUrl}/concerns`);
  }

  getConcern(id: number | string) {
    return this.http.get<Concern>(`${this.baseUrl}/concerns/${id}`);
  }

  updateConcern(id: number, dto: any) {
    return this.http.put(`${this.baseUrl}/concerns/${id}`, dto);
  }
}
