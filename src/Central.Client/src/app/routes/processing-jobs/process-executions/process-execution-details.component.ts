import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { TranslateModule } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { ProcessingService } from '../services/processing.service';
import { ProcessExecution, ProcessExecutionStep, ExecutionStatus } from '../models/process.models';

@Component({
  selector: 'app-process-execution-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    MatExpansionModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button (click)="navigateBack()">
            <mat-icon>arrow_back</mat-icon>
            {{ 'back' | translate }}
          </button>
          <button mat-raised-button (click)="loadExecution()">
            <mat-icon>refresh</mat-icon>
            {{ 'refresh' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">
            {{ 'processing.loading' | translate }}
          </div>
        } @else if (execution()) {
          <div class="details-container">
            <mat-card class="summary-card">
              <mat-card-header>
                <mat-card-title>Execution Summary</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                <div class="detail-row">
                  <span class="detail-label">Status</span>
                  <mat-chip [class]="getStatusClass(execution()!.status)">
                    <mat-icon>{{ getStatusIcon(execution()!.status) }}</mat-icon>
                    {{ execution()!.status }}
                  </mat-chip>
                </div>

                <div class="detail-row">
                  <span class="detail-label">Process Definition</span>
                  <a (click)="navigateToDefinition(execution()!.processDefinitionId)" class="link">
                    Process #{{ execution()!.processDefinitionId }}
                  </a>
                </div>

                <div class="detail-row">
                  <span class="detail-label">Document</span>
                  <a (click)="navigateToDocument(execution()!.documentId)" class="link">
                    Document #{{ execution()!.documentId }}
                  </a>
                </div>

                <div class="detail-row">
                  <span class="detail-label">Started</span>
                  <span>{{ execution()!.startedAt | date:'medium' }}</span>
                </div>

                @if (execution()!.completedAt) {
                  <div class="detail-row">
                    <span class="detail-label">Completed</span>
                    <span>{{ execution()!.completedAt | date:'medium' }}</span>
                  </div>

                  <div class="detail-row">
                    <span class="detail-label">Duration</span>
                    <span>{{ getDuration(execution()!) }}</span>
                  </div>
                }

                @if (execution()!.errorMessage) {
                  <div class="error-message">
                    <mat-icon color="warn">error</mat-icon>
                    <div>
                      <strong>Error:</strong><br>
                      {{ execution()!.errorMessage }}
                    </div>
                  </div>
                }
              </mat-card-content>
            </mat-card>

            <mat-card class="steps-card">
              <mat-card-header>
                <mat-card-title>Execution Steps</mat-card-title>
              </mat-card-header>
              <mat-card-content>
                @if (execution()!.steps.length === 0) {
                  <div class="no-data">No steps recorded</div>
                } @else {
                  <div class="timeline">
                    @for (step of execution()!.steps; track step.id; let last = $last) {
                      <div class="timeline-item" [class.timeline-item-last]="last">
                        <div class="timeline-marker" [ngClass]="getStepMarkerClass(step)">
                          <mat-icon>{{ getStepIcon(step) }}</mat-icon>
                        </div>
                        <div class="timeline-content">
                          <mat-card class="step-card">
                            <mat-card-content>
                              <div class="step-header">
                                <div>
                                  <h3 class="step-name">{{ step.stepName }}</h3>
                                  <span class="step-type">{{ step.stepType }}</span>
                                </div>
                                <mat-chip [class]="getStatusClass(step.status)">
                                  {{ step.status }}
                                </mat-chip>
                              </div>

                              @if (step.startedAt) {
                                <div class="step-times">
                                  <div class="time-row">
                                    <mat-icon>schedule</mat-icon>
                                    Started: {{ step.startedAt | date:'short' }}
                                  </div>
                                  @if (step.completedAt) {
                                    <div class="time-row">
                                      <mat-icon>check_circle</mat-icon>
                                      Completed: {{ step.completedAt | date:'short' }}
                                      ({{ getStepDuration(step) }})
                                    </div>
                                  }
                                </div>
                              }

                              @if (step.errorMessage) {
                                <div class="step-error">
                                  <mat-icon color="warn">error</mat-icon>
                                  <div>
                                    <strong>Error:</strong><br>
                                    {{ step.errorMessage }}
                                  </div>
                                </div>
                              }

                              @if (step.output) {
                                <div class="step-output">
                                  <button mat-stroked-button
                                          (click)="toggleStepOutput(step.id)">
                                    <mat-icon>
                                      {{ isStepOutputVisible(step.id) ? 'expand_less' : 'expand_more' }}
                                    </mat-icon>
                                    {{ isStepOutputVisible(step.id) ? 'Hide' : 'Show' }} Output
                                  </button>

                                  @if (isStepOutputVisible(step.id)) {
                                    <pre class="output-content">{{ step.output }}</pre>
                                  }
                                </div>
                              }
                            </mat-card-content>
                          </mat-card>
                        </div>
                      </div>
                    }
                  </div>
                }
              </mat-card-content>
            </mat-card>
          </div>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .header-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-bottom: 20px;
    }

    .loading {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .details-container {
      display: grid;
      grid-template-columns: 350px 1fr;
      gap: 20px;
      align-items: start;
    }

    .summary-card {
      position: sticky;
      top: 20px;
    }

    .detail-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 0;
      border-bottom: 1px solid #e0e0e0;

      &:last-child {
        border-bottom: none;
      }
    }

    .detail-label {
      font-weight: 500;
      color: #666;
    }

    .link {
      color: #1976d2;
      cursor: pointer;
      text-decoration: none;

      &:hover {
        text-decoration: underline;
      }
    }

    .error-message {
      display: flex;
      gap: 12px;
      padding: 16px;
      background-color: #ffebee;
      border-radius: 4px;
      margin-top: 16px;
      color: #c62828;
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

    .no-data {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .timeline {
      position: relative;
      padding-left: 40px;
    }

    .timeline-item {
      position: relative;
      padding-bottom: 30px;

      &::before {
        content: '';
        position: absolute;
        left: -21px;
        top: 40px;
        bottom: 0;
        width: 2px;
        background: #e0e0e0;
      }

      &.timeline-item-last::before {
        display: none;
      }
    }

    .timeline-marker {
      position: absolute;
      left: -36px;
      top: 0;
      width: 32px;
      height: 32px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;

      &.marker-success {
        background: #4caf50;
      }

      &.marker-danger {
        background: #f44336;
      }

      &.marker-primary {
        background: #2196f3;
      }

      &.marker-warning {
        background: #ff9800;
      }

      &.marker-secondary {
        background: #9e9e9e;
      }

      mat-icon {
        font-size: 16px;
        width: 16px;
        height: 16px;
      }
    }

    .timeline-content {
      flex: 1;
    }

    .step-card {
      margin-bottom: 0;
    }

    .step-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 16px;
    }

    .step-name {
      margin: 0 0 4px 0;
      font-size: 16px;
      font-weight: 500;
    }

    .step-type {
      font-size: 12px;
      color: #666;
    }

    .step-times {
      margin-top: 12px;
      padding-top: 12px;
      border-top: 1px solid #e0e0e0;
    }

    .time-row {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
      color: #666;
      margin: 4px 0;

      mat-icon {
        font-size: 16px;
        width: 16px;
        height: 16px;
      }
    }

    .step-error {
      display: flex;
      gap: 12px;
      padding: 12px;
      background-color: #ffebee;
      border-radius: 4px;
      margin-top: 12px;
      color: #c62828;
    }

    .step-output {
      margin-top: 12px;

      button {
        margin-bottom: 12px;
      }
    }

    .output-content {
      max-height: 300px;
      overflow: auto;
      font-size: 12px;
      background-color: #f5f5f5;
      padding: 16px;
      border-radius: 4px;
      margin: 0;
    }
  `]
})
export class ProcessExecutionDetailsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly processingService = inject(ProcessingService);

  execution = signal<ProcessExecution | null>(null);
  loading = signal(false);
  executionId = 0;
  visibleStepOutputs = new Set<number>();

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.executionId = +id;
      this.loadExecution();
    }
  }

  loadExecution(): void {
    this.loading.set(true);

    this.processingService.getProcessExecution(this.executionId).subscribe({
      next: (execution) => {
        this.execution.set(execution);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.loading.set(false);
        console.error('Error loading execution:', err);
      }
    });
  }

  navigateBack(): void {
    this.router.navigate(['/processing-jobs/executions']);
  }

  navigateToDefinition(id: number): void {
    this.router.navigate(['/processing-jobs/definitions', id]);
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

  getStepMarkerClass(step: ProcessExecutionStep): string {
    switch (step.status) {
      case ExecutionStatus.Completed: return 'marker-success';
      case ExecutionStatus.Running: return 'marker-primary';
      case ExecutionStatus.Failed: return 'marker-danger';
      case ExecutionStatus.Pending: return 'marker-warning';
      default: return 'marker-secondary';
    }
  }

  getStepIcon(step: ProcessExecutionStep): string {
    switch (step.status) {
      case ExecutionStatus.Completed: return 'check';
      case ExecutionStatus.Running: return 'sync';
      case ExecutionStatus.Failed: return 'close';
      case ExecutionStatus.Pending: return 'schedule';
      default: return 'help';
    }
  }

  getDuration(execution: ProcessExecution): string {
    if (!execution.startedAt || !execution.completedAt) return '—';

    const start = new Date(execution.startedAt).getTime();
    const end = new Date(execution.completedAt).getTime();
    const durationMs = end - start;
    const seconds = Math.floor(durationMs / 1000);

    if (seconds < 60) return `${seconds}s`;

    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  }

  getStepDuration(step: ProcessExecutionStep): string {
    if (!step.startedAt || !step.completedAt) return '—';

    const start = new Date(step.startedAt).getTime();
    const end = new Date(step.completedAt).getTime();
    const durationMs = end - start;
    const seconds = Math.floor(durationMs / 1000);

    if (seconds < 60) return `${seconds}s`;

    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  }

  toggleStepOutput(stepId: number): void {
    if (this.visibleStepOutputs.has(stepId)) {
      this.visibleStepOutputs.delete(stepId);
    } else {
      this.visibleStepOutputs.add(stepId);
    }
  }

  isStepOutputVisible(stepId: number): boolean {
    return this.visibleStepOutputs.has(stepId);
  }
}
