import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map } from 'rxjs';

import { Menu } from '@core';
import { User } from './interface';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  protected readonly http = inject(HttpClient);

  login(username: string, password: string, rememberMe = false) {
    return this.http.post<User>('/api/auth/login', { username, password, rememberMe });
  }

  logout() {
    return this.http.post<void>('/api/auth/logout', {});
  }

  user() {
    return this.http.get<User>('/api/auth/me');
  }

  menu() {
    return this.http.get<{ menu: Menu[] }>('data/menu.json?_t=' + Date.now()).pipe(map(res => res.menu));
  }
}
