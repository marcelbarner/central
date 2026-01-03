import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { ProcessingService } from '../services/processing.service';
import { ProcessExecution, ExecutionStatus } from '../models/process.models';

@Component({
  selector: 'app-process-executions-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    MatChipsModule,
    MatProgressBarModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button (click)="loadExecutions()">
            <mat-icon>refresh</mat-icon>
            {{ 'refresh' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'processing.loading_executions' | translate }}</div>
        } @else if (executions().length === 0) {
          <div class="no-data">
            <mat-icon>history</mat-icon>
            <p>{{ 'processing.no_executions' | translate }}</p>
          </div>
        } @else {
          <table mat-table [dataSource]="executions()" class="executions-table">
            <ng-container matColumnDef="id">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.id' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                <strong>#{{ execution.id }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="process">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.process' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                Process #{{ execution.processDefinitionId }}
              </td>
            </ng-container>

            <ng-container matColumnDef="document">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.document' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                <a (click)="navigateToDocument(execution.documentId)" class="document-link">
                  Document #{{ execution.documentId }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.status' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                <mat-chip [class]="getStatusClass(execution.status)">
                  <mat-icon>{{ getStatusIcon(execution.status) }}</mat-icon>
                  {{ execution.status }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="started">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.started' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                {{ execution.startedAt | date:'short' }}
              </td>
            </ng-container>

            <ng-container matColumnDef="progress">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.progress' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                <div class="progress-container">
                  <mat-progress-bar
                    [value]="getProgressPercentage(execution)"
                    [color]="getProgressColor(execution)"
                    mode="determinate"
                  ></mat-progress-bar>
                  <span class="progress-text">
                    {{ getCompletedSteps(execution) }}/{{ execution.steps.length }}
                  </span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let execution">
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'processing.view_details' | translate"
                  (click)="navigateToDetails(execution.id)"
                >
                  <mat-icon>visibility</mat-icon>
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

    .loading {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .no-data {
      text-align: center;
      padding: 60px 20px;
      color: #666;

      mat-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        color: #ccc;
      }

      p {
        margin: 16px 0 24px;
        font-size: 16px;
      }
    }

    .executions-table {
      width: 100%;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.02);
      cursor: pointer;
    }

    .document-link {
      cursor: pointer;
      color: #1976d2;
      text-decoration: none;

      &:hover {
        text-decoration: underline;
      }
    }

    mat-chip {
      &.status-completed {
        background-color: #4caf50;
        color: white;
      }

      &.status-running {
        background-color: #2196f3;
        color: white;
      }

      &.status-failed {
        background-color: #f44336;
        color: white;
      }

      &.status-pending {
        background-color: #ff9800;
        color: white;
      }

      &.status-cancelled {
        background-color: #9e9e9e;
        color: white;
      }

      mat-icon {
        font-size: 16px;
        width: 16px;
        height: 16px;
        margin-right: 4px;
      }
    }

    .progress-container {
      display: flex;
      align-items: center;
      gap: 8px;
      min-width: 120px;

      mat-progress-bar {
        flex: 1;
      }

      .progress-text {
        font-size: 12px;
        color: #666;
        white-space: nowrap;
      }
    }
  `]
})
export class ProcessExecutionsListComponent implements OnInit {
  private readonly processingService = inject(ProcessingService);
  private readonly router = inject(Router);

  executions = signal<ProcessExecution[]>([]);
  loading = signal(false);

  displayedColumns = ['id', 'process', 'document', 'status', 'started', 'progress', 'actions'];

  ngOnInit(): void {
    this.loadExecutions();
  }

  loadExecutions(): void {
    this.loading.set(true);

    this.processingService.getProcessExecutions().subscribe({
      next: (executions) => {
        this.executions.set(executions);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.loading.set(false);
        console.error('Error loading executions:', err);
      }
    });
  }

  navigateToDetails(id: number): void {
    this.router.navigate(['/processing-jobs/executions', id]);
  }

  navigateToDocument(id: number): void {
    this.router.navigate(['/documents', id]);
  }

  getStatusClass(status: ExecutionStatus): string {
    switch (status) {
      case ExecutionStatus.Completed: return 'status-completed';
      case ExecutionStatus.Running: return 'status-running';
      case ExecutionStatus.Failed: return 'status-failed';
      case ExecutionStatus.Pending: return 'status-pending';
      case ExecutionStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }

  getStatusIcon(status: ExecutionStatus): string {
    switch (status) {
      case ExecutionStatus.Completed: return 'check_circle';
      case ExecutionStatus.Running: return 'sync';
      case ExecutionStatus.Failed: return 'error';
      case ExecutionStatus.Pending: return 'schedule';
      case ExecutionStatus.Cancelled: return 'cancel';
      default: return 'help';
    }
  }

  getProgressPercentage(execution: ProcessExecution): number {
    if (execution.steps.length === 0) return 0;
    return (this.getCompletedSteps(execution) / execution.steps.length) * 100;
  }

  getCompletedSteps(execution: ProcessExecution): number {
    return execution.steps.filter(s => s.status === ExecutionStatus.Completed).length;
  }

  getProgressColor(execution: ProcessExecution): 'primary' | 'accent' | 'warn' {
    switch (execution.status) {
      case ExecutionStatus.Completed: return 'accent';
      case ExecutionStatus.Failed: return 'warn';
      default: return 'primary';
    }
  }
}
