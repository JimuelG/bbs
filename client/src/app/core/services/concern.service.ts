import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Concern } from '../../shared/models/concern';
import { ConcernParms } from '../../shared/models/concernParams';
import { Pagination } from '../../shared/models/pagination';

@Injectable({
  providedIn: 'root',
})
export class ConcernService {
  private http = inject(HttpClient)
  private baseUrl = environment.apiUrl;

  createConcern(dto: any) {
    return this.http.post<{ message: string, id: number }>(`${this.baseUrl}/concerns`, dto);
  }

  getAllConcerns(concernParams: ConcernParms) {
    let params = new HttpParams()
      .set('pageSize', concernParams.pageSize)
      .set('pageIndex', concernParams.pageIndex)

    if (concernParams.sort) {
      params = params.append('sort', concernParams.sort);
    }

    if (concernParams.priority) {
      params = params.append('priority', concernParams.priority);
    }

    if (concernParams.search) {
      params = params.append('search', concernParams.search);
    }

    return this.http.get<Pagination<Concern>>(`${this.baseUrl}/concerns`, { params });
  }

  uploadPhoto(concernId: number, file: File) {
    const formData = new FormData();
    formData.append('file', file);
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
