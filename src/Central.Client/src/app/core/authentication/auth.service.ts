import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, iif, map, merge, of, share, switchMap, tap, catchError } from 'rxjs';
import { isEmptyObject } from './helpers';
import { User } from './interface';
import { LoginService } from './login.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly loginService = inject(LoginService);

  private user$ = new BehaviorSubject<User>({});
  private authenticated$ = new BehaviorSubject<boolean>(false);

  private change$ = this.authenticated$.pipe(
    switchMap(() => this.assignUser()),
    share()
  );

  init() {
    return new Promise<void>(resolve => {
      this.loadUser().subscribe(() => resolve());
    });
  }

  change() {
    return this.change$;
  }

  check() {
    return this.authenticated$.value;
  }

  login(username: string, password: string, rememberMe = false) {
    return this.loginService.login(username, password, rememberMe).pipe(
      tap(user => {
        this.user$.next(user);
        this.authenticated$.next(true);
      }),
      map(() => true),
      catchError(() => of(false))
    );
  }

  logout() {
    return this.loginService.logout().pipe(
      tap(() => {
        this.user$.next({});
        this.authenticated$.next(false);
      }),
      map(() => true),
      catchError(() => of(true))
    );
  }

  user() {
    return this.user$.pipe(share());
  }

  menu() {
    return iif(() => this.check(), this.loginService.menu(), of([]));
  }

  private loadUser() {
    return this.loginService.user().pipe(
      tap(user => {
        this.user$.next(user);
        this.authenticated$.next(true);
      }),
      catchError(() => {
        this.user$.next({});
        this.authenticated$.next(false);
        return of({});
      })
    );
  }

  private assignUser() {
    if (!this.check()) {
      return of({}).pipe(tap(user => this.user$.next(user)));
    }

    if (!isEmptyObject(this.user$.getValue())) {
      return of(this.user$.getValue());
    }

    return this.loginService.user().pipe(
      tap(user => this.user$.next(user)),
      catchError(() => {
        this.authenticated$.next(false);
        return of({});
      })
    );
  }
}
