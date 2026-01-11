import { Injectable, inject } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.model';
import { TaskService } from '../../services/task.service';

export namespace TasksActions {
  export class Load {
    static readonly type = '[Tasks] Load';
  }

  export class Loaded {
    static readonly type = '[Tasks] Loaded';
    constructor(public tasks: Task[]) {}
  }

  export class Add {
    static readonly type = '[Tasks] Add';
    constructor(public request: CreateTaskRequest) {}
  }

  export class Added {
    static readonly type = '[Tasks] Added';
    constructor(public task: Task) {}
  }

  export class Update {
    static readonly type = '[Tasks] Update';
    constructor(
      public id: number,
      public request: UpdateTaskRequest
    ) {}
  }

  export class Updated {
    static readonly type = '[Tasks] Updated';
    constructor(public task: Task) {}
  }

  export class Delete {
    static readonly type = '[Tasks] Delete';
    constructor(public id: number) {}
  }

  export class Deleted {
    static readonly type = '[Tasks] Deleted';
    constructor(public id: number) {}
  }

  export class Error {
    static readonly type = '[Tasks] Error';
    constructor(public error: string) {}
  }

  export class ClearError {
    static readonly type = '[Tasks] Clear Error';
  }
}

export interface TasksStateModel {
  tasks: Task[];
  loading: boolean;
  error: string | null;
}

@State<TasksStateModel>({
  name: 'tasks',
  defaults: {
    tasks: [],
    loading: false,
    error: null
  }
})
@Injectable()
export class TasksState {
  private readonly taskService = inject(TaskService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  @Selector()
  static tasks(state: TasksStateModel): Task[] {
    return state.tasks;
  }

  @Selector()
  static loading(state: TasksStateModel): boolean {
    return state.loading;
  }

  @Selector()
  static error(state: TasksStateModel): string | null {
    return state.error;
  }

  @Action(TasksActions.Load)
  load(ctx: StateContext<TasksStateModel>) {
    ctx.patchState({ loading: true, error: null });
    return this.taskService.getAll().pipe(
      tap(tasks => ctx.dispatch(new TasksActions.Loaded(tasks))),
      catchError(error => {
        ctx.dispatch(new TasksActions.Error(error.message));
        return of(null);
      })
    );
  }

  @Action(TasksActions.Loaded)
  loaded(ctx: StateContext<TasksStateModel>, action: TasksActions.Loaded) {
    ctx.patchState({
      tasks: action.tasks,
      loading: false
    });
  }

  @Action(TasksActions.Add)
  add(ctx: StateContext<TasksStateModel>, action: TasksActions.Add) {
    ctx.patchState({ loading: true, error: null });
    return this.taskService.create(action.request).pipe(
      tap(task => {
        ctx.dispatch(new TasksActions.Added(task));
        this.snackBar.open(
          this.translate.instant('tasks.created'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new TasksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('tasks.createError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(TasksActions.Added)
  added(ctx: StateContext<TasksStateModel>, action: TasksActions.Added) {
    const state = ctx.getState();
    const tasks = [...state.tasks, action.task].sort((a, b) =>
      a.name.localeCompare(b.name)
    );
    ctx.patchState({
      tasks,
      loading: false
    });
  }

  @Action(TasksActions.Update)
  update(ctx: StateContext<TasksStateModel>, action: TasksActions.Update) {
    ctx.patchState({ loading: true, error: null });
    return this.taskService.update(action.id, action.request).pipe(
      tap(task => {
        ctx.dispatch(new TasksActions.Updated(task));
        this.snackBar.open(
          this.translate.instant('tasks.updated'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new TasksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('tasks.updateError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(TasksActions.Updated)
  updated(ctx: StateContext<TasksStateModel>, action: TasksActions.Updated) {
    const state = ctx.getState();
    const tasks = state.tasks
      .map(t => (t.id === action.task.id ? action.task : t))
      .sort((a, b) => a.name.localeCompare(b.name));
    ctx.patchState({
      tasks,
      loading: false
    });
  }

  @Action(TasksActions.Delete)
  delete(ctx: StateContext<TasksStateModel>, action: TasksActions.Delete) {
    ctx.patchState({ loading: true, error: null });
    return this.taskService.delete(action.id).pipe(
      tap(() => {
        ctx.dispatch(new TasksActions.Deleted(action.id));
        this.snackBar.open(
          this.translate.instant('tasks.deleted'),
          this.translate.instant('common.close'),
          { duration: 3000 }
        );
      }),
      catchError(error => {
        ctx.dispatch(new TasksActions.Error(error.message));
        this.snackBar.open(
          this.translate.instant('tasks.deleteError'),
          this.translate.instant('common.close'),
          { duration: 5000 }
        );
        return of(null);
      })
    );
  }

  @Action(TasksActions.Deleted)
  deleted(ctx: StateContext<TasksStateModel>, action: TasksActions.Deleted) {
    const state = ctx.getState();
    const tasks = state.tasks.filter(t => t.id !== action.id);
    ctx.patchState({
      tasks,
      loading: false
    });
  }

  @Action(TasksActions.Error)
  error(ctx: StateContext<TasksStateModel>, action: TasksActions.Error) {
    ctx.patchState({
      error: action.error,
      loading: false
    });
  }

  @Action(TasksActions.ClearError)
  clearError(ctx: StateContext<TasksStateModel>) {
    ctx.patchState({ error: null });
  }
}
