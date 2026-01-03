import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ProcessDefinition,
  CreateProcessDefinitionRequest,
  UpdateProcessDefinitionRequest,
  ProcessExecution,
  CreateProcessExecutionRequest
} from '../models/process.models';

@Injectable({
  providedIn: 'root'
})
export class ProcessingService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api';

  // Process Definitions
  getProcessDefinitions(): Observable<ProcessDefinition[]> {
    return this.http.get<ProcessDefinition[]>(`${this.apiUrl}/process-definitions`);
  }

  getProcessDefinition(id: number): Observable<ProcessDefinition> {
    return this.http.get<ProcessDefinition>(`${this.apiUrl}/process-definitions/${id}`);
  }

  createProcessDefinition(request: CreateProcessDefinitionRequest): Observable<ProcessDefinition> {
    return this.http.post<ProcessDefinition>(`${this.apiUrl}/process-definitions`, request);
  }

  updateProcessDefinition(id: number, request: UpdateProcessDefinitionRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/process-definitions/${id}`, request);
  }

  deleteProcessDefinition(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/process-definitions/${id}`);
  }

  // Process Executions
  getProcessExecutions(): Observable<ProcessExecution[]> {
    return this.http.get<ProcessExecution[]>(`${this.apiUrl}/process-executions`);
  }

  getProcessExecution(id: number): Observable<ProcessExecution> {
    return this.http.get<ProcessExecution>(`${this.apiUrl}/process-executions/${id}`);
  }

  createProcessExecution(request: CreateProcessExecutionRequest): Observable<ProcessExecution> {
    return this.http.post<ProcessExecution>(`${this.apiUrl}/process-executions`, request);
  }

  getDocumentExecutions(documentId: number): Observable<ProcessExecution[]> {
    return this.http.get<ProcessExecution[]>(`${this.apiUrl}/documents/${documentId}/executions`);
  }
}
