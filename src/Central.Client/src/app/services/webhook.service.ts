import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Webhook, CreateWebhookRequest, UpdateWebhookRequest } from '../models/webhook.model';

@Injectable({
  providedIn: 'root'
})
export class WebhookService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/webhooks';

  getAll(): Observable<Webhook[]> {
    return this.http.get<Webhook[]>(this.apiUrl);
  }

  getById(id: number): Observable<Webhook> {
    return this.http.get<Webhook>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateWebhookRequest): Observable<Webhook> {
    return this.http.post<Webhook>(this.apiUrl, request);
  }

  update(request: UpdateWebhookRequest): Observable<Webhook> {
    return this.http.put<Webhook>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
