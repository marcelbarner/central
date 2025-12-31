import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Document, CreateDocumentRequest, UpdateDocumentRequest } from '../../shared/models/document.model';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/documents';

  getAll(): Observable<Document[]> {
    return this.http.get<Document[]>(this.apiUrl);
  }

  getById(id: number): Observable<Document> {
    return this.http.get<Document>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateDocumentRequest): Observable<Document> {
    const formData = new FormData();
    formData.append('title', request.title);
    if (request.documentDate) {
      formData.append('documentDate', request.documentDate);
    }
    if (request.content) {
      formData.append('content', request.content);
    }
    if (request.documentTypeId) {
      formData.append('documentTypeId', request.documentTypeId.toString());
    }
    if (request.correspondentId) {
      formData.append('correspondentId', request.correspondentId.toString());
    }
    formData.append('originalFile', request.originalFile);

    return this.http.post<Document>(this.apiUrl, formData);
  }

  update(request: UpdateDocumentRequest): Observable<Document> {
    return this.http.put<Document>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  downloadOriginal(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/files/original`, {
      responseType: 'blob',
    });
  }

  downloadArchive(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/files/archive`, {
      responseType: 'blob',
    });
  }

  getThumbnail(id: number): string {
    return `${this.apiUrl}/${id}/files/thumbnail`;
  }

  getArchiveUrl(id: number): string {
    return `${this.apiUrl}/${id}/files/archive`;
  }
}
