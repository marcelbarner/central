import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Correspondent, CreateCorrespondentRequest, UpdateCorrespondentRequest } from '../models/correspondent.model';

@Injectable({
  providedIn: 'root'
})
export class CorrespondentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/correspondents';

  getAll(): Observable<Correspondent[]> {
    return this.http.get<Correspondent[]>(this.apiUrl);
  }

  getById(id: number): Observable<Correspondent> {
    return this.http.get<Correspondent>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateCorrespondentRequest): Observable<Correspondent> {
    return this.http.post<Correspondent>(this.apiUrl, request);
  }

  update(request: UpdateCorrespondentRequest): Observable<Correspondent> {
    return this.http.put<Correspondent>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
