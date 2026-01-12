import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Store } from '@ngxs/store';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { Task } from '../../models/task.model';
import { TasksState, TasksActions } from '../../core/states/tasks.state';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-tasks-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    MatChipsModule,
    MatCheckboxModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          @if (selection.hasValue()) {
            <button mat-raised-button color="warn" (click)="deleteSelected()">
              <mat-icon>delete</mat-icon>
              {{ 'delete_selected' | translate }} ({{ selection.selected.length }})
            </button>
          }
          <button mat-raised-button color="primary" (click)="createTask()">
            <mat-icon>add</mat-icon>
            Create Task
          </button>
        </div>

        @if (loading()) {
          <div class="loading">Loading tasks...</div>
        } @else if (tasks().length === 0) {
          <div class="no-data">
            <mat-icon>psychology</mat-icon>
            <p>No tasks available</p>
            <button mat-raised-button color="primary" (click)="createTask()">
              Create your first task
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="tasks()" class="tasks-table">
            <!-- Checkbox Column -->
            <ng-container matColumnDef="select">
              <th mat-header-cell *matHeaderCellDef>
                <mat-checkbox
                  (change)="$event ? toggleAllRows() : null"
                  [checked]="selection.hasValue() && isAllSelected()"
                  [indeterminate]="selection.hasValue() && !isAllSelected()"
                />
              </th>
              <td mat-cell *matCellDef="let row">
                <mat-checkbox
                  (click)="$event.stopPropagation()"
                  (change)="$event ? toggleRow(row) : null"
                  [checked]="selection.isSelected(row)"
                />
              </td>
            </ng-container>

            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let task">
                <strong>{{ task.name }}</strong>
                @if (task.description) {
                  <div class="text-secondary text-sm">{{ task.description }}</div>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let task">
                <mat-chip [color]="task.taskType === 'AzureOpenAI' ? 'primary' : 'accent'">
                  {{ task.taskType === 'AzureOpenAI' ? 'Azure OpenAI' : 'Document Intelligence' }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let task">
                <mat-chip [color]="task.enabled ? 'primary' : ''">
                  {{ task.enabled ? 'Enabled' : 'Disabled' }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="configuration">
              <th mat-header-cell *matHeaderCellDef>Configuration</th>
              <td mat-cell *matCellDef="let task">
                <div class="text-sm">
                  @if (task.configuration.azureModelOrDeployment) {
                    <div>Model: {{ task.configuration.azureModelOrDeployment }}</div>
                  }
                  @if (task.configuration.azureEndpoint) {
                    <div class="text-secondary">{{ getEndpointDisplay(task.configuration.azureEndpoint) }}</div>
                  }
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let task">
                <button
                  mat-icon-button
                  color="primary"
                  matTooltip="Edit"
                  (click)="editTask(task)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  matTooltip="Delete"
                  (click)="deleteTask(task)"
                >
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns" class="table-row"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .header-actions {
      display: flex;
      justify-content: flex-end;
      align-items: center;
      gap: 12px;
      margin-bottom: 20px;
    }

    .loading,
    .no-data {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 40px;
      text-align: center;
    }

    .no-data mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #ccc;
      margin-bottom: 16px;
    }

    .tasks-table {
      width: 100%;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.04);
    }

    .text-secondary {
      color: rgba(0, 0, 0, 0.6);
    }

    .text-sm {
      font-size: 0.875rem;
    }
  `],
})
export class TasksListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);
  private readonly router = inject(Router);

  tasks = this.store.selectSignal(TasksState.tasks);
  loading = this.store.selectSignal(TasksState.loading);
  selection = new SelectionModel<Task>(true, []);
  displayedColumns = ['select', 'name', 'type', 'status', 'configuration', 'actions'];

  ngOnInit() {
    this.store.dispatch(new TasksActions.Load());
  }

  createTask() {
    this.router.navigate(['/tasks/new']);
  }

  editTask(task: Task) {
    this.router.navigate(['/tasks', task.id, 'edit']);
  }

  deleteTask(task: Task) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('tasks.confirm_delete_single', { name: task.name }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.store.dispatch(new TasksActions.Delete(task.id));
      }
    });
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.tasks().length;
    return numSelected === numRows && numRows > 0;
  }

  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.tasks().forEach(row => this.selection.select(row));
    }
  }

  toggleRow(row: Task): void {
    this.selection.toggle(row);
  }

  async deleteSelected(): Promise<void> {
    const selectedTasks = this.selection.selected;
    const count = selectedTasks.length;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('tasks.confirm_delete_multiple', { count }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    for (const task of selectedTasks) {
      this.store.dispatch(new TasksActions.Delete(task.id));
    }

    this.selection.clear();
  }

  getEndpointDisplay(endpoint: string): string {
    try {
      const url = new URL(endpoint);
      return url.hostname;
    } catch {
      return endpoint;
    }
  }
}
