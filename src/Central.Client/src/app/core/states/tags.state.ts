import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { catchError, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Tag, CreateTagRequest, UpdateTagRequest } from '../../models/tag.model';
import { TagService } from '../../services/tag.service';

/**
 * Tags State Management with NGXS
 *
 * This state manages the tags collection with the following features:
 * - Loading state indicator (loading: boolean)
 * - Loaded state indicator (loaded: boolean)
 * - Error handling (error: any | null)
 * - Full CRUD operations (Load, Add, Update, Delete)
 *
 * Action Pattern:
 * - Each operation has three actions: Action, ActionSuccess (Added/Loaded/etc), ActionError
 * - Example: Add -> Added -> AddError
 *
 * Usage in components:
 * ```typescript
 * @Select(TagsState.tags) tags$: Observable<Tag[]>;
 * @Select(TagsState.loading) loading$: Observable<boolean>;
 * @Select(TagsState.error) error$: Observable<any>;
 *
 * // Dispatch actions
 * this.store.dispatch(new TagsActions.Load());
 * this.store.dispatch(new TagsActions.Add(payload));
 * ```
 */

// Actions
export namespace TagsActions {
  export class Load {
    static readonly type = '[Tags] Load';
  }

  export class Loaded {
    static readonly type = '[Tags] Loaded';
    constructor(public tags: Tag[]) {}
  }

  export class LoadError {
    static readonly type = '[Tags] Load Error';
    constructor(public error: any) {}
  }

  export class Add {
    static readonly type = '[Tags] Add';
    constructor(public payload: CreateTagRequest) {}
  }

  export class Added {
    static readonly type = '[Tags] Added';
    constructor(public tag: Tag) {}
  }

  export class AddError {
    static readonly type = '[Tags] Add Error';
    constructor(public error: any) {}
  }

  export class Update {
    static readonly type = '[Tags] Update';
    constructor(public payload: UpdateTagRequest) {}
  }

  export class Updated {
    static readonly type = '[Tags] Updated';
    constructor(public tag: Tag) {}
  }

  export class UpdateError {
    static readonly type = '[Tags] Update Error';
    constructor(public error: any) {}
  }

  export class Delete {
    static readonly type = '[Tags] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Tags] Deleted';
    constructor(public id: number) {}
  }

  export class DeleteError {
    static readonly type = '[Tags] Delete Error';
    constructor(public error: any) {}
  }

  export class ClearError {
    static readonly type = '[Tags] Clear Error';
  }
}

// State Model
export interface TagsStateModel {
  tags: Tag[];
  loaded: boolean;
  loading: boolean;
  error: any | null;
}

// State
@State<TagsStateModel>({
  name: 'tags',
  defaults: {
    tags: [],
    loaded: false,
    loading: false,
    error: null,
  },
})
@Injectable()
export class TagsState {
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  constructor(private tagService: TagService) {}

  // Selectors
  @Selector()
  static tags(state: TagsStateModel): Tag[] {
    return state.tags;
  }

  @Selector()
  static loaded(state: TagsStateModel): boolean {
    return state.loaded;
  }

  @Selector()
  static loading(state: TagsStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: TagsStateModel): any | null {
    return state.error;
  }

  @Selector()
  static getById(state: TagsStateModel) {
    return (id: number) => {
      return state.tags.find(tag => tag.id === id);
    };
  }

  // Actions
  @Action(TagsActions.Load)
  load(ctx: StateContext<TagsStateModel>) {
    ctx.patchState({ loading: true, error: null });

    return this.tagService.getAll().pipe(
      tap(tags => {
        ctx.dispatch(new TagsActions.Loaded(tags));
      }),
      catchError(error => {
        ctx.dispatch(new TagsActions.LoadError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(TagsActions.Loaded)
  loaded(ctx: StateContext<TagsStateModel>, action: TagsActions.Loaded) {
    ctx.patchState({
      tags: action.tags,
      loaded: true,
      loading: false,
      error: null,
    });
  }

  @Action(TagsActions.LoadError)
  loadError(ctx: StateContext<TagsStateModel>, action: TagsActions.LoadError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('tags.failed_to_load').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.Add)
  add(ctx: StateContext<TagsStateModel>, action: TagsActions.Add) {
    ctx.patchState({ loading: true, error: null });

    return this.tagService.create(action.payload).pipe(
      tap(tag => {
        ctx.dispatch(new TagsActions.Added(tag));
      }),
      catchError(error => {
        ctx.dispatch(new TagsActions.AddError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(TagsActions.Added)
  added(ctx: StateContext<TagsStateModel>, action: TagsActions.Added) {
    const state = ctx.getState();
    // Insert new tag and keep alphabetical order
    const tags = [...state.tags, action.tag].sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      tags,
      loading: false,
      error: null,
    });

    this.translate.get('tags.created_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.AddError)
  addError(ctx: StateContext<TagsStateModel>, action: TagsActions.AddError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('tags.failed_to_create').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.Update)
  update(ctx: StateContext<TagsStateModel>, action: TagsActions.Update) {
    ctx.patchState({ loading: true, error: null });

    return this.tagService.update(action.payload).pipe(
      tap(tag => {
        ctx.dispatch(new TagsActions.Updated(tag));
      }),
      catchError(error => {
        ctx.dispatch(new TagsActions.UpdateError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(TagsActions.Updated)
  updated(ctx: StateContext<TagsStateModel>, action: TagsActions.Updated) {
    const state = ctx.getState();
    // Update tag and maintain alphabetical order
    const tags = state.tags
      .map((tag: Tag) => (tag.id === action.tag.id ? action.tag : tag))
      .sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      tags,
      loading: false,
      error: null,
    });

    this.translate.get('tags.updated_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.UpdateError)
  updateError(ctx: StateContext<TagsStateModel>, action: TagsActions.UpdateError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('tags.failed_to_update').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.Delete)
  delete(ctx: StateContext<TagsStateModel>, action: TagsActions.Delete) {
    ctx.patchState({ loading: true, error: null });

    return this.tagService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new TagsActions.Deleted(action.id));
      }),
      catchError(error => {
        ctx.dispatch(new TagsActions.DeleteError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(TagsActions.Deleted)
  deleted(ctx: StateContext<TagsStateModel>, action: TagsActions.Deleted) {
    const state = ctx.getState();
    const tags = state.tags.filter((tag: Tag) => tag.id !== action.id);
    ctx.patchState({
      tags,
      loading: false,
      error: null,
    });

    this.translate.get('tags.deleted_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.DeleteError)
  deleteError(ctx: StateContext<TagsStateModel>, action: TagsActions.DeleteError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('tags.failed_to_delete').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(TagsActions.ClearError)
  clearError(ctx: StateContext<TagsStateModel>) {
    ctx.patchState({ error: null });
  }
}
