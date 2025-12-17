export interface DocumentFile {
  fileName: string;
  filePath: string;
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
  tagIds: number[];
}

export interface CreateDocumentRequest {
  title: string;
  documentDate: string | null;
  content: string | null;
  originalFile: File;
  tagIds?: number[];
}

export interface UpdateDocumentRequest {
  id: number;
  title: string;
  documentDate: string | null;
  content: string | null;
  tagIds?: number[];
}
