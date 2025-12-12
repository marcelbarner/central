export interface User {
  [prop: string]: any;

  id?: number | string | null;
  username?: string;
  displayName?: string;
  email?: string;
  avatar?: string;
  roles?: string[];
  lastLoginAt?: string | null;
}

export interface Token {
  [prop: string]: any;

  access_token: string;
  token_type?: string;
  expires_in?: number;
  exp?: number;
  refresh_token?: string;
}
