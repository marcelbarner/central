import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { TranslateModule } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';
import { TaskDialogComponent } from './task-dialog.component';

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
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button color="primary" (click)="openDialog()">
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
            <button mat-raised-button color="primary" (click)="openDialog()">
              Create your first task
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="tasks()" class="tasks-table">
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
                  (click)="openDialog(task)"
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
  private readonly taskService = inject(TaskService);
  private readonly dialog = inject(MatDialog);

  tasks = signal<Task[]>([]);
  loading = signal(true);
  displayedColumns = ['name', 'type', 'status', 'configuration', 'actions'];

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.loading.set(true);
    this.taskService.getAll().subscribe({
      next: (tasks) => {
        this.tasks.set(tasks);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading tasks:', error);
        this.loading.set(false);
      },
    });
  }

  openDialog(task?: Task) {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '800px',
      data: task,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        if (task) {
          this.taskService.update(task.id, result).subscribe(() => this.loadTasks());
        } else {
          this.taskService.create(result).subscribe(() => this.loadTasks());
        }
      }
    });
  }

  deleteTask(task: Task) {
    if (confirm(`Are you sure you want to delete task "${task.name}"?`)) {
      this.taskService.delete(task.id).subscribe(() => this.loadTasks());
    }
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
