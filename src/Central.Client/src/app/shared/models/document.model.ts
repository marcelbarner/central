export interface DocumentFile {
  fileName: string;
  filePath: string;
}

export enum DocumentState {
  Imported = 'Imported',
  Processing = 'Processing',
  Processed = 'Processed',
  Review = 'Review',
  Approved = 'Approved',
  Failed = 'Failed'
}

export interface Document {
  id: number;
  title: string;
  documentDate: string | null;
  content: string | null;
  originalFile: DocumentFile | null;
  archiveFile: DocumentFile | null;
  thumbnail: DocumentFile | null;
  added: string;
  updated: string;
  addedById: number | null;
  updatedById: number | null;
  documentTypeId: number | null;
  correspondentId: number | null;
  contractId: number | null;
  state: DocumentState;
  tagIds: number[];
}

export interface CreateDocumentRequest {
  title: string;
  documentDate: string | null;
  content: string | null;
  originalFile: File;
  documentTypeId?: number | null;
  correspondentId?: number | null;
  tagIds?: number[];
}

export interface UpdateDocumentRequest {
  id: number;
  title: string;
  documentDate: string | null;
  content: string | null;
  documentTypeId?: number | null;
  correspondentId?: number | null;
  contractId?: number | null;
  state: string;
  tagIds?: number[];
}
