export interface Contract {
  id: number;
  name: string;
  description?: string;
  state: ContractState;
  correspondentId?: number;
  customerId?: string;
  contractId?: string;
  created: string;
  updated: string;
}

export enum ContractState {
  Draft = 'Draft',
  Active = 'Active',
  Expired = 'Expired',
  Terminated = 'Terminated'
}

export interface CreateContractRequest {
  name: string;
  description?: string;
  state: string;
  correspondentId?: number;
  customerId?: string;
  contractId?: string;
}

export interface UpdateContractRequest {
  id: number;
  name: string;
  description?: string;
  state: string;
  correspondentId?: number;
  customerId?: string;
  contractId?: string;
}

export interface AssignContractToDocumentRequest {
  contractId: number;
  documentId: number;
  syncCorrespondent: boolean;
}
