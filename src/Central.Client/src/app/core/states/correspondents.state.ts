import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Correspondent } from '../../models/correspondent.model';
import { CorrespondentService } from '../../services/correspondent.service';

export namespace CorrespondentsActions {
  export class Load {
    static readonly type = '[Correspondents] Load';
  }

  export class Loaded {
    static readonly type = '[Correspondents] Loaded';
    constructor(public correspondents: Correspondent[]) {}
  }

  export class Add {
    static readonly type = '[Correspondents] Add';
    constructor(public name: string, public description?: string) {}
  }

  export class Added {
    static readonly type = '[Correspondents] Added';
    constructor(public correspondent: Correspondent) {}
  }

  export class Update {
    static readonly type = '[Correspondents] Update';
    constructor(public id: number, public name: string, public description?: string) {}
  }

  export class Updated {
    static readonly type = '[Correspondents] Updated';
    constructor(public correspondent: Correspondent) {}
  }

  export class Delete {
    static readonly type = '[Correspondents] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Correspondents] Deleted';
    constructor(public id: number) {}
  }

  export class Error {
    static readonly type = '[Correspondents] Error';
    constructor(public error: string) {}
  }

  export class ClearError {
    static readonly type = '[Correspondents] Clear Error';
  }
}

export interface CorrespondentsStateModel {
  correspondents: Correspondent[];
  loading: boolean;
  error: string | null;
}

@State<CorrespondentsStateModel>({
  name: 'correspondents',
  defaults: {
    correspondents: [],
    loading: false,
    error: null
  }
})
@Injectable()
export class CorrespondentsState {
  private readonly correspondentService = inject(CorrespondentService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  @Selector()
  static correspondents(state: CorrespondentsStateModel): Correspondent[] {
    return state.correspondents;
  }

  @Selector()
  static loading(state: CorrespondentsStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: CorrespondentsStateModel): string | null {
    return state.error;
  }

  @Action(CorrespondentsActions.Load)
  load(ctx: StateContext<CorrespondentsStateModel>) {
    ctx.patchState({ loading: true, error: null });
    return this.correspondentService.getAll().pipe(
      tap(correspondents => ctx.dispatch(new CorrespondentsActions.Loaded(correspondents))),
      catchError(error => {
        ctx.dispatch(new CorrespondentsActions.Error(error.message));
        return of(null);
      })
    );
  }

  @Action(CorrespondentsActions.Loaded)
  loaded(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Loaded) {
    ctx.patchState({
      correspondents: action.correspondents,
      loading: false
    });
  }

  @Action(CorrespondentsActions.Add)
  add(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Add) {
    ctx.patchState({ loading: true, error: null });
    return this.correspondentService.create({ name: action.name, description: action.description }).pipe(
      tap(correspondent => {
        ctx.dispatch(new CorrespondentsActions.Added(correspondent));
        this.snackBar.open(
          this.translate.instant('correspondents.created'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new CorrespondentsActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('correspondents.createError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(CorrespondentsActions.Added)
  added(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Added) {
    const state = ctx.getState();
    const correspondents = [...state.correspondents, action.correspondent].sort((a, b) =>
      a.name.localeCompare(b.name)
    );
    ctx.patchState({
      correspondents,
      loading: false
    });
  }

  @Action(CorrespondentsActions.Update)
  update(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Update) {
    ctx.patchState({ loading: true, error: null });
    return this.correspondentService.update({
      id: action.id,
      name: action.name,
      description: action.description
    }).pipe(
      tap(correspondent => {
        ctx.dispatch(new CorrespondentsActions.Updated(correspondent));
        this.snackBar.open(
          this.translate.instant('correspondents.updated'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new CorrespondentsActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('correspondents.updateError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(CorrespondentsActions.Updated)
  updated(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Updated) {
    const state = ctx.getState();
    const correspondents = state.correspondents
      .map(c => c.id === action.correspondent.id ? action.correspondent : c)
      .sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      correspondents,
      loading: false
    });
  }

  @Action(CorrespondentsActions.Delete)
  delete(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Delete) {
    ctx.patchState({ loading: true, error: null });
    return this.correspondentService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new CorrespondentsActions.Deleted(action.id));
        this.snackBar.open(
          this.translate.instant('correspondents.deleted'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new CorrespondentsActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('correspondents.deleteError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(CorrespondentsActions.Deleted)
  deleted(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Deleted) {
    const state = ctx.getState();
    ctx.patchState({
      correspondents: state.correspondents.filter(c => c.id !== action.id),
      loading: false
    });
  }

  @Action(CorrespondentsActions.Error)
  error(ctx: StateContext<CorrespondentsStateModel>, action: CorrespondentsActions.Error) {
    ctx.patchState({
      loading: false,
      error: action.error
    });
  }

  @Action(CorrespondentsActions.ClearError)
  clearError(ctx: StateContext<CorrespondentsStateModel>) {
    ctx.patchState({ error: null });
  }
}
