import { Component, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { TasksState, TasksActions } from '../../core/states/tasks.state';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';

export interface ExecuteTaskDialogData {
  documentIds: number[];
  documentCount: number;
}

@Component({
  selector: 'app-execute-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>Execute Task</h2>
    <mat-dialog-content>
      @if (loadingTasks()) {
        <div class="loading">
          <mat-spinner diameter="40" />
          <p>Loading tasks...</p>
        </div>
      } @else {
        <p class="dialog-description">
          Select a task to execute on
          {{ data.documentCount === 1 ? 'this document' : data.documentCount + ' documents' }}.
        </p>

        @if (enabledTasks().length === 0) {
          <div class="warning-message">
            <mat-icon>warning</mat-icon>
            <span>No enabled tasks available. Please create and enable tasks first.</span>
          </div>
        } @else {
          <form [formGroup]="form">
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Task</mat-label>
              <mat-select formControlName="taskId" required>
                @for (task of enabledTasks(); track task.id) {
                  <mat-option [value]="task.id">
                    {{ task.name }}
                    <span class="task-type">({{ getTaskTypeDisplay(task.taskType) }})</span>
                  </mat-option>
                }
              </mat-select>
              @if (form.get('taskId')?.hasError('required')) {
                <mat-error>Task is required</mat-error>
              }
            </mat-form-field>

            @if (selectedTask) {
              <div class="task-info">
                <h4>Task Information</h4>
                <p><strong>Type:</strong> {{ getTaskTypeDisplay(selectedTask.taskType) }}</p>
                @if (selectedTask.description) {
                  <p><strong>Description:</strong> {{ selectedTask.description }}</p>
                }
              </div>
            }
          </form>
        }
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()" [disabled]="executing">Cancel</button>
      <button
        mat-raised-button
        color="primary"
        (click)="onExecute()"
        [disabled]="!form.valid || executing || enabledTasks().length === 0"
      >
        @if (executing) {
          <mat-spinner diameter="20" style="display: inline-block; margin-right: 8px;" />
          Executing...
        } @else {
          Execute
        }
      </button>
    </mat-dialog-actions>
  `,
  styles: [
    `
      .loading {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 40px;
        text-align: center;

        p {
          margin-top: 16px;
        }
      }

      .warning-message {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 12px;
        background-color: #fff3cd;
        border: 1px solid #ffc107;
        border-radius: 4px;
        color: #856404;

        mat-icon {
          font-size: 20px;
          width: 20px;
          height: 20px;
        }
      }

      .task-type {
        font-size: 0.875rem;
        color: rgba(0, 0, 0, 0.6);
        margin-left: 8px;
      }

      .task-info {
        margin-top: 16px;
        padding: 16px;
        background-color: #f5f5f5;
        border-radius: 4px;

        h4 {
          margin: 0 0 8px 0;
          font-size: 14px;
          font-weight: 500;
        }

        p {
          margin: 4px 0;
          font-size: 14px;
        }
      }

      mat-dialog-content {
        min-width: 400px;
        max-height: 70vh;
        overflow-y: auto;
      }
    `,
  ],
})
export class ExecuteTaskDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ExecuteTaskDialogComponent>);
  private readonly store = inject(Store);
  private readonly taskService = inject(TaskService); // Keep for execute() method
  readonly data = inject<ExecuteTaskDialogData>(MAT_DIALOG_DATA);

  form!: FormGroup;
  availableTasks = this.store.selectSignal(TasksState.tasks);
  loadingTasks = this.store.selectSignal(TasksState.loading);
  enabledTasks = computed(() => this.availableTasks().filter(t => t.enabled));
  executing = false;
  selectedTask: Task | null = null;

  ngOnInit() {
    this.store.dispatch(new TasksActions.Load());

    this.form = this.fb.group({
      taskId: [null, Validators.required],
    });

    this.form.get('taskId')?.valueChanges.subscribe(taskId => {
      this.selectedTask = this.availableTasks().find(t => t.id === taskId) || null;
    });
  }

  getTaskTypeDisplay(taskType: string): string {
    return taskType === 'AzureOpenAI' ? 'Azure OpenAI' : 'Document Intelligence';
  }

  onExecute() {
    if (this.form.valid) {
      this.executing = true;
      const taskId = this.form.value.taskId;

      // Execute task for each document
      const executions = this.data.documentIds.map(documentId =>
        this.taskService.execute(taskId, { documentId })
      );

      // Wait for all executions to complete (or fail)
      Promise.allSettled(
        executions.map(obs => obs.toPromise())
      ).then(results => {
        const successful = results.filter(r => r.status === 'fulfilled').length;
        const failed = results.filter(r => r.status === 'rejected').length;

        this.dialogRef.close({
          success: successful > 0,
          successful,
          failed,
          total: this.data.documentIds.length,
        });
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
