import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Tag, CreateTagRequest, UpdateTagRequest } from '../models/tag.model';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/tags';

  getAll(): Observable<Tag[]> {
    return this.http.get<Tag[]>(this.apiUrl);
  }

  getById(id: number): Observable<Tag> {
    return this.http.get<Tag>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTagRequest): Observable<Tag> {
    return this.http.post<Tag>(this.apiUrl, request);
  }

  update(request: UpdateTagRequest): Observable<Tag> {
    return this.http.put<Tag>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
