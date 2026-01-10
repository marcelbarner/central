import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskExecution,
  ExecuteTaskRequest,
} from '../models/task.model';

@Injectable({
  providedIn: 'root',
})
export class TaskService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/tasks';

  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl);
  }

  getById(id: number): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  execute(id: number, request: ExecuteTaskRequest): Observable<TaskExecution> {
    return this.http.post<TaskExecution>(`${this.apiUrl}/${id}/execute`, request);
  }

  getExecutions(id: number): Observable<TaskExecution[]> {
    return this.http.get<TaskExecution[]>(`${this.apiUrl}/${id}/executions`);
  }

  getExecutionById(id: number): Observable<TaskExecution> {
    return this.http.get<TaskExecution>(`/api/task-executions/${id}`);
  }
}
