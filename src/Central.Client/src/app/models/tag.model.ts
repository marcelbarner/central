export interface Tag {
  id: number;
  name: string;
  description?: string;
  created: string;
  updated: string;
}

export interface CreateTagRequest {
  name: string;
  description?: string;
}

export interface UpdateTagRequest {
  id: number;
  name: string;
  description?: string;
}
