import { HttpErrorResponse, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '@core/authentication';
import { catchError, tap, throwError } from 'rxjs';
import { BASE_URL, hasHttpScheme } from './base-url-interceptor';

/**
 * Cookie authentication interceptor.
 * Ensures cookies are sent with requests and handles authentication errors.
 */
export function tokenInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
  const router = inject(Router);
  const baseUrl = inject(BASE_URL, { optional: true });
  const authService = inject(AuthService);

  const includeBaseUrl = (url: string) => {
    if (!baseUrl) {
      return false;
    }
    return new RegExp(`^${baseUrl.replace(/\/$/, '')}`, 'i').test(url);
  };

  const shouldIncludeCredentials = (url: string) => !hasHttpScheme(url) || includeBaseUrl(url);

  const handler = () => {
    if (req.url.includes('/auth/logout')) {
      router.navigateByUrl('/auth/login');
    }

    if (router.url.includes('/auth/login')) {
      router.navigateByUrl('/dashboard');
    }
  };

  // Clone request with credentials enabled for same-origin or baseUrl requests
  const clonedReq = shouldIncludeCredentials(req.url)
    ? req.clone({ withCredentials: true })
    : req;

  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.logout();
        router.navigateByUrl('/auth/login');
      }
      return throwError(() => error);
    }),
    tap(() => handler())
  );
}
