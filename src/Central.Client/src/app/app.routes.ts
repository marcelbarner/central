import { Routes } from '@angular/router';
import { authGuard } from '@core';
import { AdminLayout } from '@theme/admin-layout/admin-layout';
import { AuthLayout } from '@theme/auth-layout/auth-layout';

export const routes: Routes = [
  {
    path: '',
    component: AdminLayout,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./routes/dashboard/dashboard').then(m => m.Dashboard),
      },
      {
        path: 'documents',
        loadComponent: () =>
          import('./routes/documents/documents-list').then(m => m.DocumentsList),
      },
      {
        path: 'documents/:id',
        loadComponent: () =>
          import('./routes/documents/document-details').then(m => m.DocumentDetails),
      },
      {
        path: 'tags',
        loadComponent: () =>
          import('./routes/tags-overview/tags-list.component').then(m => m.TagsListComponent),
      },
      {
        path: '403',
        loadComponent: () => import('./routes/sessions/error-403').then(m => m.Error403),
      },
      {
        path: '404',
        loadComponent: () => import('./routes/sessions/error-404').then(m => m.Error404),
      },
      {
        path: '500',
        loadComponent: () => import('./routes/sessions/error-500').then(m => m.Error500),
      },
    ],
  },
  {
    path: 'auth',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./routes/sessions/login/login').then(m => m.Login),
      },
      {
        path: 'register',
        loadComponent: () => import('./routes/sessions/register/register').then(m => m.Register),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
