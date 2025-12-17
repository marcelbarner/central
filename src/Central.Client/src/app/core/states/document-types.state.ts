import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { catchError, tap } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { DocumentType, CreateDocumentTypeRequest, UpdateDocumentTypeRequest } from '../../models/document-type.model';
import { DocumentTypeService } from '../../services/document-type.service';

/**
 * Document Types State Management with NGXS
 *
 * This state manages the document types collection with the following features:
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
 * @Select(DocumentTypesState.documentTypes) documentTypes$: Observable<DocumentType[]>;
 * @Select(DocumentTypesState.loading) loading$: Observable<boolean>;
 * @Select(DocumentTypesState.error) error$: Observable<any>;
 *
 * // Dispatch actions
 * this.store.dispatch(new DocumentTypesActions.Load());
 * this.store.dispatch(new DocumentTypesActions.Add(payload));
 * ```
 */

// Actions
export namespace DocumentTypesActions {
  export class Load {
    static readonly type = '[DocumentTypes] Load';
  }

  export class Loaded {
    static readonly type = '[DocumentTypes] Loaded';
    constructor(public documentTypes: DocumentType[]) {}
  }

  export class LoadError {
    static readonly type = '[DocumentTypes] Load Error';
    constructor(public error: any) {}
  }

  export class Add {
    static readonly type = '[DocumentTypes] Add';
    constructor(public name: string, public description?: string) {}
  }

  export class Added {
    static readonly type = '[DocumentTypes] Added';
    constructor(public documentType: DocumentType) {}
  }

  export class AddError {
    static readonly type = '[DocumentTypes] Add Error';
    constructor(public error: any) {}
  }

  export class Update {
    static readonly type = '[DocumentTypes] Update';
    constructor(public id: number, public name: string, public description?: string) {}
  }

  export class Updated {
    static readonly type = '[DocumentTypes] Updated';
    constructor(public documentType: DocumentType) {}
  }

  export class UpdateError {
    static readonly type = '[DocumentTypes] Update Error';
    constructor(public error: any) {}
  }

  export class Delete {
    static readonly type = '[DocumentTypes] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[DocumentTypes] Deleted';
    constructor(public id: number) {}
  }

  export class DeleteError {
    static readonly type = '[DocumentTypes] Delete Error';
    constructor(public error: any) {}
  }

  export class ClearError {
    static readonly type = '[DocumentTypes] Clear Error';
  }
}

// State Model
export interface DocumentTypesStateModel {
  documentTypes: DocumentType[];
  loaded: boolean;
  loading: boolean;
  error: any | null;
}

// State
@State<DocumentTypesStateModel>({
  name: 'documentTypes',
  defaults: {
    documentTypes: [],
    loaded: false,
    loading: false,
    error: null,
  },
})
@Injectable()
export class DocumentTypesState {
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  constructor(private documentTypeService: DocumentTypeService) {}

  // Selectors
  @Selector()
  static documentTypes(state: DocumentTypesStateModel): DocumentType[] {
    return state.documentTypes;
  }

  @Selector()
  static loaded(state: DocumentTypesStateModel): boolean {
    return state.loaded;
  }

  @Selector()
  static loading(state: DocumentTypesStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: DocumentTypesStateModel): any | null {
    return state.error;
  }

  @Selector()
  static getById(state: DocumentTypesStateModel) {
    return (id: number) => {
      return state.documentTypes.find(documentType => documentType.id === id);
    };
  }

  // Actions
  @Action(DocumentTypesActions.Load)
  load(ctx: StateContext<DocumentTypesStateModel>) {
    ctx.patchState({ loading: true, error: null });

    return this.documentTypeService.getAll().pipe(
      tap(documentTypes => {
        ctx.dispatch(new DocumentTypesActions.Loaded(documentTypes));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentTypesActions.LoadError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentTypesActions.Loaded)
  loaded(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Loaded) {
    ctx.patchState({
      documentTypes: action.documentTypes,
      loaded: true,
      loading: false,
      error: null,
    });
  }

  @Action(DocumentTypesActions.LoadError)
  loadError(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.LoadError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documentTypes.failed_to_load').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.Add)
  add(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Add) {
    ctx.patchState({ loading: true, error: null });

    return this.documentTypeService.create({ name: action.name, description: action.description }).pipe(
      tap(documentType => {
        ctx.dispatch(new DocumentTypesActions.Added(documentType));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentTypesActions.AddError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentTypesActions.Added)
  added(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Added) {
    const state = ctx.getState();
    // Insert new document type and keep alphabetical order
    const documentTypes = [...state.documentTypes, action.documentType].sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      documentTypes,
      loading: false,
      error: null,
    });

    this.translate.get('documentTypes.created_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.AddError)
  addError(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.AddError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documentTypes.failed_to_create').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.Update)
  update(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Update) {
    ctx.patchState({ loading: true, error: null });

    return this.documentTypeService.update({ id: action.id, name: action.name, description: action.description }).pipe(
      tap(documentType => {
        ctx.dispatch(new DocumentTypesActions.Updated(documentType));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentTypesActions.UpdateError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentTypesActions.Updated)
  updated(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Updated) {
    const state = ctx.getState();
    // Update document type and maintain alphabetical order
    const documentTypes = state.documentTypes
      .map((documentType: DocumentType) => (documentType.id === action.documentType.id ? action.documentType : documentType))
      .sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      documentTypes,
      loading: false,
      error: null,
    });

    this.translate.get('documentTypes.updated_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.UpdateError)
  updateError(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.UpdateError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documentTypes.failed_to_update').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.Delete)
  delete(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Delete) {
    ctx.patchState({ loading: true, error: null });

    return this.documentTypeService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new DocumentTypesActions.Deleted(action.id));
      }),
      catchError(error => {
        ctx.dispatch(new DocumentTypesActions.DeleteError(error));
        return throwError(() => error);
      })
    );
  }

  @Action(DocumentTypesActions.Deleted)
  deleted(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.Deleted) {
    const state = ctx.getState();
    const documentTypes = state.documentTypes.filter((documentType: DocumentType) => documentType.id !== action.id);
    ctx.patchState({
      documentTypes,
      loading: false,
      error: null,
    });

    this.translate.get('documentTypes.deleted_successfully').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.DeleteError)
  deleteError(ctx: StateContext<DocumentTypesStateModel>, action: DocumentTypesActions.DeleteError) {
    ctx.patchState({
      loading: false,
      error: action.error,
    });

    this.translate.get('documentTypes.failed_to_delete').subscribe(msg => {
      this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
    });
  }

  @Action(DocumentTypesActions.ClearError)
  clearError(ctx: StateContext<DocumentTypesStateModel>) {
    ctx.patchState({ error: null });
  }
}
