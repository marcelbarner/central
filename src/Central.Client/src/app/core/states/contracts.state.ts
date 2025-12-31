import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Contract } from '../../models/contract.model';
import { ContractService } from '../../services/contract.service';

export namespace ContractsActions {
  export class Load {
    static readonly type = '[Contracts] Load';
  }

  export class Loaded {
    static readonly type = '[Contracts] Loaded';
    constructor(public contracts: Contract[]) {}
  }

  export class Add {
    static readonly type = '[Contracts] Add';
    constructor(
      public name: string,
      public state: string,
      public description?: string,
      public correspondentId?: number,
      public customerId?: string,
      public contractId?: string
    ) {}
  }

  export class Added {
    static readonly type = '[Contracts] Added';
    constructor(public contract: Contract) {}
  }

  export class Update {
    static readonly type = '[Contracts] Update';
    constructor(
      public id: number,
      public name: string,
      public state: string,
      public description?: string,
      public correspondentId?: number,
      public customerId?: string,
      public contractId?: string
    ) {}
  }

  export class Updated {
    static readonly type = '[Contracts] Updated';
    constructor(public contract: Contract) {}
  }

  export class Delete {
    static readonly type = '[Contracts] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Contracts] Deleted';
    constructor(public id: number) {}
  }

  export class Error {
    static readonly type = '[Contracts] Error';
    constructor(public error: string) {}
  }

  export class ClearError {
    static readonly type = '[Contracts] Clear Error';
  }
}

export interface ContractsStateModel {
  contracts: Contract[];
  loading: boolean;
  error: string | null;
}

@State<ContractsStateModel>({
  name: 'contracts',
  defaults: {
    contracts: [],
    loading: false,
    error: null
  }
})
@Injectable()
export class ContractsState {
  private readonly contractService = inject(ContractService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  @Selector()
  static contracts(state: ContractsStateModel): Contract[] {
    return state.contracts;
  }

  @Selector()
  static loading(state: ContractsStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: ContractsStateModel): string | null {
    return state.error;
  }

  @Action(ContractsActions.Load)
  load(ctx: StateContext<ContractsStateModel>) {
    ctx.patchState({ loading: true, error: null });
    return this.contractService.getAll().pipe(
      tap(contracts => ctx.dispatch(new ContractsActions.Loaded(contracts))),
      catchError(error => {
        ctx.dispatch(new ContractsActions.Error(error.message));
        return of(null);
      })
    );
  }

  @Action(ContractsActions.Loaded)
  loaded(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Loaded) {
    ctx.patchState({
      contracts: action.contracts,
      loading: false
    });
  }

  @Action(ContractsActions.Add)
  add(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Add) {
    ctx.patchState({ loading: true, error: null });
    return this.contractService
      .create({
        name: action.name,
        description: action.description,
        state: action.state,
        correspondentId: action.correspondentId,
        customerId: action.customerId,
        contractId: action.contractId
      })
      .pipe(
        tap(contract => {
          ctx.dispatch(new ContractsActions.Added(contract));
          this.snackBar.open(
            this.translate.instant('contracts.created'),
            this.translate.instant('common.close'),
            { duration: 3000 }
          );
        }),
        catchError(error => {
          ctx.dispatch(new ContractsActions.Error(error.message));
          this.snackBar.open(
            this.translate.instant('contracts.createError'),
            this.translate.instant('common.close'),
            { duration: 5000 }
          );
          return of(null);
        })
      );
  }

  @Action(ContractsActions.Added)
  added(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Added) {
    const state = ctx.getState();
    const contracts = [...state.contracts, action.contract].sort((a, b) =>
      a.name.localeCompare(b.name)
    );
    ctx.patchState({
      contracts,
      loading: false
    });
  }

  @Action(ContractsActions.Update)
  update(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Update) {
    ctx.patchState({ loading: true, error: null });
    return this.contractService
      .update({
        id: action.id,
        name: action.name,
        description: action.description,
        state: action.state,
        correspondentId: action.correspondentId,
        customerId: action.customerId,
        contractId: action.contractId
      })
      .pipe(
        tap(contract => {
          ctx.dispatch(new ContractsActions.Updated(contract));
          this.snackBar.open(
            this.translate.instant('contracts.updated'),
            this.translate.instant('common.close'),
            { duration: 3000 }
          );
        }),
        catchError(error => {
          ctx.dispatch(new ContractsActions.Error(error.message));
          this.snackBar.open(
            this.translate.instant('contracts.updateError'),
            this.translate.instant('common.close'),
            { duration: 5000 }
          );
          return of(null);
        })
      );
  }

  @Action(ContractsActions.Updated)
  updated(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Updated) {
    const state = ctx.getState();
    const contracts = state.contracts
      .map(c => (c.id === action.contract.id ? action.contract : c))
      .sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      contracts,
      loading: false
    });
  }

  @Action(ContractsActions.Delete)
  delete(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Delete) {
    ctx.patchState({ loading: true, error: null });
    return this.contractService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new ContractsActions.Deleted(action.id));
        this.snackBar.open(
          this.translate.instant('contracts.deleted'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new ContractsActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('contracts.deleteError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(ContractsActions.Deleted)
  deleted(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Deleted) {
    const state = ctx.getState();
    const contracts = state.contracts.filter(c => c.id !== action.id);
    ctx.patchState({
      contracts,
      loading: false
    });
  }

  @Action(ContractsActions.Error)
  error(ctx: StateContext<ContractsStateModel>, action: ContractsActions.Error) {
    ctx.patchState({
      error: action.error,
      loading: false
    });
  }

  @Action(ContractsActions.ClearError)
  clearError(ctx: StateContext<ContractsStateModel>) {
    ctx.patchState({ error: null });
  }
}
