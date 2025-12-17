import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Webhook } from '../../models/webhook.model';
import { WebhookService } from '../../services/webhook.service';

export namespace WebhooksActions {
  export class Load {
    static readonly type = '[Webhooks] Load';
  }

  export class Loaded {
    static readonly type = '[Webhooks] Loaded';
    constructor(public webhooks: Webhook[]) {}
  }

  export class Add {
    static readonly type = '[Webhooks] Add';
    constructor(public eventType: string, public url: string) {}
  }

  export class Added {
    static readonly type = '[Webhooks] Added';
    constructor(public webhook: Webhook) {}
  }

  export class Update {
    static readonly type = '[Webhooks] Update';
    constructor(public id: number, public eventType: string, public url: string) {}
  }

  export class Updated {
    static readonly type = '[Webhooks] Updated';
    constructor(public webhook: Webhook) {}
  }

  export class Delete {
    static readonly type = '[Webhooks] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Webhooks] Deleted';
    constructor(public id: number) {}
  }

  export class Error {
    static readonly type = '[Webhooks] Error';
    constructor(public error: string) {}
  }

  export class ClearError {
    static readonly type = '[Webhooks] Clear Error';
  }
}

export interface WebhooksStateModel {
  webhooks: Webhook[];
  loading: boolean;
  error: string | null;
}

@State<WebhooksStateModel>({
  name: 'webhooks',
  defaults: {
    webhooks: [],
    loading: false,
    error: null
  }
})
@Injectable()
export class WebhooksState {
  private readonly webhookService = inject(WebhookService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  @Selector()
  static webhooks(state: WebhooksStateModel): Webhook[] {
    return state.webhooks;
  }

  @Selector()
  static loading(state: WebhooksStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: WebhooksStateModel): string | null {
    return state.error;
  }

  @Action(WebhooksActions.Load)
  load(ctx: StateContext<WebhooksStateModel>) {
    ctx.patchState({ loading: true, error: null });
    return this.webhookService.getAll().pipe(
      tap(webhooks => ctx.dispatch(new WebhooksActions.Loaded(webhooks))),
      catchError(error => {
        ctx.dispatch(new WebhooksActions.Error(error.message));
        return of(null);
      })
    );
  }

  @Action(WebhooksActions.Loaded)
  loaded(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Loaded) {
    ctx.patchState({
      webhooks: action.webhooks,
      loading: false
    });
  }

  @Action(WebhooksActions.Add)
  add(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Add) {
    ctx.patchState({ loading: true, error: null });
    return this.webhookService.create({ eventType: action.eventType, url: action.url }).pipe(
      tap(webhook => {
        ctx.dispatch(new WebhooksActions.Added(webhook));
        this.snackBar.open(
          this.translate.instant('webhooks.created'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new WebhooksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('webhooks.createError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(WebhooksActions.Added)
  added(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Added) {
    const state = ctx.getState();
    const webhooks = [...state.webhooks, action.webhook].sort((a, b) =>
      new Date(b.created).getTime() - new Date(a.created).getTime()
    );
    ctx.patchState({
      webhooks,
      loading: false
    });
  }

  @Action(WebhooksActions.Update)
  update(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Update) {
    ctx.patchState({ loading: true, error: null });
    return this.webhookService.update({
      id: action.id,
      eventType: action.eventType,
      url: action.url
    }).pipe(
      tap(webhook => {
        ctx.dispatch(new WebhooksActions.Updated(webhook));
        this.snackBar.open(
          this.translate.instant('webhooks.updated'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new WebhooksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('webhooks.updateError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(WebhooksActions.Updated)
  updated(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Updated) {
    const state = ctx.getState();
    const webhooks = state.webhooks
      .map(w => w.id === action.webhook.id ? action.webhook : w)
      .sort((a, b) => new Date(b.created).getTime() - new Date(a.created).getTime());
    ctx.patchState({
      webhooks,
      loading: false
    });
  }

  @Action(WebhooksActions.Delete)
  delete(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Delete) {
    ctx.patchState({ loading: true, error: null });
    return this.webhookService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new WebhooksActions.Deleted(action.id));
        this.snackBar.open(
          this.translate.instant('webhooks.deleted'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new WebhooksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('webhooks.deleteError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(WebhooksActions.Deleted)
  deleted(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Deleted) {
    const state = ctx.getState();
    ctx.patchState({
      webhooks: state.webhooks.filter(w => w.id !== action.id),
      loading: false
    });
  }

  @Action(WebhooksActions.Error)
  error(ctx: StateContext<WebhooksStateModel>, action: WebhooksActions.Error) {
    ctx.patchState({
      loading: false,
      error: action.error
    });
  }

  @Action(WebhooksActions.ClearError)
  clearError(ctx: StateContext<WebhooksStateModel>) {
    ctx.patchState({ error: null });
  }
}
