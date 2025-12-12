import { Injectable } from '@angular/core';
import { EMPTY, of } from 'rxjs';
import { admin, LoginService, Menu } from '@core';
import { map } from 'rxjs/operators';

/**
 * You should delete this file in the real APP.
 */
@Injectable()
export class FakeLoginService extends LoginService {
  login() {
    return of(admin);
  }

  logout() {
    return EMPTY;
  }

  user() {
    return of(admin);
  }

  menu() {
    return this.http
      .get<{ menu: Menu[] }>('data/menu.json?_t=' + Date.now())
      .pipe(map(res => res.menu));
  }
}
