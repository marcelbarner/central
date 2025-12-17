export interface DocumentType {
  id: number;
  name: string;
  description?: string;
  created: string;
  updated: string;
}

export interface CreateDocumentTypeRequest {
  name: string;
  description?: string;
}

export interface UpdateDocumentTypeRequest {
  id: number;
  name: string;
  description?: string;
}
