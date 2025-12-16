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
}

export interface CreateDocumentRequest {
  title: string;
  documentDate: string | null;
  content: string | null;
  originalFile: File;
}

export interface UpdateDocumentRequest {
  id: number;
  title: string;
  documentDate: string | null;
  content: string | null;
}
