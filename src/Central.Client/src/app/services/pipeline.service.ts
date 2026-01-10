import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Pipeline,
  CreatePipelineRequest,
  UpdatePipelineRequest,
  PipelineExecution,
  ExecutePipelineRequest,
} from '../models/pipeline.model';

@Injectable({
  providedIn: 'root',
})
export class PipelineService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/pipelines';

  getAll(): Observable<Pipeline[]> {
    return this.http.get<Pipeline[]>(this.apiUrl);
  }

  getById(id: number): Observable<Pipeline> {
    return this.http.get<Pipeline>(`${this.apiUrl}/${id}`);
  }

  create(request: CreatePipelineRequest): Observable<Pipeline> {
    return this.http.post<Pipeline>(this.apiUrl, request);
  }

  update(id: number, request: UpdatePipelineRequest): Observable<Pipeline> {
    return this.http.put<Pipeline>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  execute(id: number, request: ExecutePipelineRequest): Observable<PipelineExecution> {
    return this.http.post<PipelineExecution>(`${this.apiUrl}/${id}/execute`, request);
  }

  getExecutions(id: number): Observable<PipelineExecution[]> {
    return this.http.get<PipelineExecution[]>(`${this.apiUrl}/${id}/executions`);
  }

  getExecutionById(id: number): Observable<PipelineExecution> {
    return this.http.get<PipelineExecution>(`/api/pipeline-executions/${id}`);
  }
}
