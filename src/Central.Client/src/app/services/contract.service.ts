import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  Contract,
  CreateContractRequest,
  UpdateContractRequest,
  AssignContractToDocumentRequest
} from '../models/contract.model';

@Injectable({
  providedIn: 'root'
})
export class ContractService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/contracts';

  getAll(): Observable<Contract[]> {
    return this.http.get<Contract[]>(this.apiUrl);
  }

  getById(id: number): Observable<Contract> {
    return this.http.get<Contract>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateContractRequest): Observable<Contract> {
    return this.http.post<Contract>(this.apiUrl, request);
  }

  update(request: UpdateContractRequest): Observable<Contract> {
    return this.http.put<Contract>(`${this.apiUrl}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  assignToDocument(request: AssignContractToDocumentRequest): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${request.contractId}/assign-to-document`,
      request
    );
  }
}
