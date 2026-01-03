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
        loadComponent: () => import('./routes/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'documents',
        loadComponent: () =>
          import('./routes/documents/documents-list').then((m) => m.DocumentsList),
      },
      {
        path: 'documents/:id',
        loadComponent: () =>
          import('./routes/documents/document-details').then((m) => m.DocumentDetails),
      },
      {
        path: 'document-types',
        loadComponent: () =>
          import('./routes/document-types/document-types-list.component').then(
            (m) => m.DocumentTypesListComponent,
          ),
      },
      {
        path: 'correspondents',
        loadComponent: () =>
          import('./routes/correspondents/correspondents-list.component').then(
            (m) => m.CorrespondentsListComponent,
          ),
      },
      {
        path: 'contracts',
        loadComponent: () =>
          import('./routes/contracts/contracts-list.component').then(
            (m) => m.ContractsListComponent,
          ),
      },
      {
        path: 'contracts/:id',
        loadComponent: () =>
          import('./routes/contracts/contract-details.component').then(
            (m) => m.ContractDetailsComponent,
          ),
      },
      {
        path: 'webhooks',
        loadComponent: () =>
          import('./routes/webhooks/webhooks-list.component').then((m) => m.WebhooksListComponent),
      },
      {
        path: 'tags',
        loadComponent: () =>
          import('./routes/tags-overview/tags-list.component').then((m) => m.TagsListComponent),
      },
      {
        path: 'processing-jobs',
        children: [
          {
            path: '',
            redirectTo: 'definitions',
            pathMatch: 'full',
          },
          {
            path: 'definitions',
            loadComponent: () =>
              import('./routes/processing-jobs/process-definitions/process-definitions-list.component').then(
                (m) => m.ProcessDefinitionsListComponent,
              ),
            title: 'Process Definitions',
          },
          {
            path: 'definitions/new',
            loadComponent: () =>
              import('./routes/processing-jobs/process-definitions/process-definition-edit.component').then(
                (m) => m.ProcessDefinitionEditComponent,
              ),
            title: 'Create Process Definition',
          },
          {
            path: 'definitions/:id',
            loadComponent: () =>
              import('./routes/processing-jobs/process-definitions/process-definition-edit.component').then(
                (m) => m.ProcessDefinitionEditComponent,
              ),
            title: 'Edit Process Definition',
          },
          {
            path: 'executions',
            loadComponent: () =>
              import('./routes/processing-jobs/process-executions/process-executions-list.component').then(
                (m) => m.ProcessExecutionsListComponent,
              ),
            title: 'Process Executions',
          },
          {
            path: 'executions/:id',
            loadComponent: () =>
              import('./routes/processing-jobs/process-executions/process-execution-details.component').then(
                (m) => m.ProcessExecutionDetailsComponent,
              ),
            title: 'Execution Details',
          },
        ],
      },
      {
        path: '403',
        loadComponent: () => import('./routes/sessions/error-403').then((m) => m.Error403),
      },
      {
        path: '404',
        loadComponent: () => import('./routes/sessions/error-404').then((m) => m.Error404),
      },
      {
        path: '500',
        loadComponent: () => import('./routes/sessions/error-500').then((m) => m.Error500),
      },
    ],
  },
  {
    path: 'auth',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./routes/sessions/login/login').then((m) => m.Login),
      },
      {
        path: 'register',
        loadComponent: () => import('./routes/sessions/register/register').then((m) => m.Register),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
