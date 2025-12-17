export interface Correspondent {
  id: number;
  name: string;
  description?: string;
  created: string;
  updated: string;
}

export interface CreateCorrespondentRequest {
  name: string;
  description?: string;
}

export interface UpdateCorrespondentRequest {
  id: number;
  name: string;
  description?: string;
}
