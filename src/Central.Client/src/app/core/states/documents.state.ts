import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { catchError, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Document } from '../../shared/models/document.model';
import { DocumentService } from '../../routes/documents/document.service';

/**
 * Documents State Management with NGXS
 *
 * This state manages the documents collection with the following features:
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
 * @Select(DocumentsState.documents) documents$: Observable<Document[]>;
 * @Select(DocumentsState.loading) loading$: Observable<boolean>;
 * @Select(DocumentsState.error) error$: Observable<any>;
 *
 * // Dispatch actions
 * this.store.dispatch(new DocumentsActions.Load());
 * this.store.dispatch(new DocumentsActions.Add(payload));
 * ```
 */

// Actions
export namespace DocumentsActions {
  export class Load {
    static readonly type = '[Documents] Load';
  }

  export class Loaded {
    static readonly type = '[Documents] Loaded';
    constructor(public documents: Document[]) {}
  }

  export class LoadError {
    static readonly type = '[Documents] Load Error';
    constructor(public error: any) {}
  }

  export class Add {
    static readonly type = '[Documents] Add';
    constructor(
      public payload: {
        title: string;
        documentDate: string | null;
        content: string | null;
        originalFile: File;
        documentTypeId?: number | null;
        correspondentId?: number | null;
        tagIds?: number[];
      }
    ) {}
  }

  export class Added {
    static readonly type = '[Documents] Added';
    constructor(public document: Document) {}
  }

  export class AddError {
    static readonly type = '[Documents] Add Error';
    constructor(public error: any) {}
  }

  export class Update {
    static readonly type = '[Documents] Update';
    constructor(
      public payload: {
        id: number;
        title: string;
        documentDate: string | null;
        content: string | null;
        documentTypeId?: number | null;
        correspondentId?: number | null;
        tagIds?: number[];
      }
    ) {}
  }

  export class Updated {
    static readonly type = '[Documents] Updated';
    constructor(public document: Document) {}
  }

  export class UpdateError {
    static readonly type = '[Documents] Update Error';
    constructor(public error: any) {}
  }

  export class Delete {
    static readonly type = '[Documents] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Documents] Deleted';
    constructor(public id: number) {}
  }

  export class DeleteError {
    static readonly type = '[Documents] Delete Error';
    constructor(public error: any) {}
  }

  export class ClearError {
    static readonly type = '[Documents] Clear Error';
  }
}

// State Model
export interface DocumentsStateModel {
  documents: Document[];
  loaded: boolean;
  loading: boolean;
  error: any | null;
}

// State
@State<DocumentsStateModel>({
  name: 'documents',
  defaults: {
    documents: [],
    loaded: false,
    loading: false,
    error: null,
  },
})
@Injectable()
export class DocumentsState {
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  constructor(private documentService: DocumentService) {}

  // Selectors
  @Selector()
  static documents(state: DocumentsStateModel): Document[] {
    return state.documents;
  }

  @Selector()
  static loaded(state: DocumentsStateModel): boolean {
    return state.loaded;
  }

  @Selector()
  static loading(state: DocumentsStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: DocumentsStateModel): any | null {
    return state.error;
  }

  @Selector()
  static getById(state: DocumentsStateModel) {
    return (id: number) => {
      return state.documents.find(doc => doc.id === id);
    };
  }

  // Actions
  @Action(DocumentsActions.Load)
  load(ctx: StateContext<DocumentsStateModel>) {
    ctx.patchState({ loading: true, error: null });

    return this.documentService.getAll().pipe(
      tap(documents => {
        ctx.dispatch(new DocumentsActions.Loaded(documents));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentsActions.LoadError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentsActions.Loaded)
  loaded(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Loaded) {
    ctx.patchState({
      documents: action.documents,
      loaded: true,
      loading: false,
      error: null,
    });
  }

  @Action(DocumentsActions.LoadError)
  loadError(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.LoadError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documents.failed_to_load').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.Add)
  add(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Add) {
    ctx.patchState({ loading: true, error: null });

    return this.documentService.create(action.payload).pipe(
      tap(document => {
        ctx.dispatch(new DocumentsActions.Added(document));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentsActions.AddError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentsActions.Added)
  added(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Added) {
    const state = ctx.getState();
    ctx.patchState({
      documents: [...state.documents, action.document],
      loading: false,
      error: null,
    });

    this.translate.get('documents.uploaded_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.AddError)
  addError(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.AddError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documents.failed_to_upload').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.Update)
  update(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Update) {
    ctx.patchState({ loading: true, error: null });

    return this.documentService.update(action.payload).pipe(
      tap(document => {
        ctx.dispatch(new DocumentsActions.Updated(document));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentsActions.UpdateError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentsActions.Updated)
  updated(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Updated) {
    const state = ctx.getState();
    const documents = state.documents.map((doc: Document) =>
      doc.id === action.document.id ? action.document : doc
    );
    ctx.patchState({
      documents,
      loading: false,
      error: null,
    });
  }

  @Action(DocumentsActions.UpdateError)
  updateError(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.UpdateError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documents.failed_to_update').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.Delete)
  delete(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Delete) {
    ctx.patchState({ loading: true, error: null });

    return this.documentService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new DocumentsActions.Deleted(action.id));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentsActions.DeleteError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentsActions.Deleted)
  deleted(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.Deleted) {
    const state = ctx.getState();
    const documents = state.documents.filter((doc: Document) => doc.id !== action.id);
    ctx.patchState({
      documents,
      loading: false,
      error: null,
    });

    this.translate.get('documents.deleted_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.DeleteError)
  deleteError(ctx: StateContext<DocumentsStateModel>, action: DocumentsActions.DeleteError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documents.failed_to_delete').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentsActions.ClearError)
  clearError(ctx: StateContext<DocumentsStateModel>) {
    ctx.patchState({ error: null });
  }
}
