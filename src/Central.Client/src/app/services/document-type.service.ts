import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { DocumentType, CreateDocumentTypeRequest, UpdateDocumentTypeRequest } from '../models/document-type.model';

@Injectable({
  providedIn: 'root',
})
export class DocumentTypeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/document-types';

  getAll(): Observable<DocumentType[]> {
    return this.http.get<DocumentType[]>(this.apiUrl);
  }

  getById(id: number): Observable<DocumentType> {
    return this.http.get<DocumentType>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateDocumentTypeRequest): Observable<DocumentType> {
    return this.http.post<DocumentType>(this.apiUrl, request);
  }

  update(request: UpdateDocumentTypeRequest): Observable<DocumentType> {
    return this.http.put<DocumentType>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
