import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { PipelineService } from '../../services/pipeline.service';
import { Pipeline } from '../../models/pipeline.model';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-pipelines-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    MatChipsModule,
    MatExpansionModule,
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
          <button mat-raised-button color="primary" (click)="createPipeline()">
            <mat-icon>add</mat-icon>
            Create Pipeline
          </button>
        </div>

        @if (loading()) {
          <div class="loading">Loading pipelines...</div>
        } @else if (pipelines().length === 0) {
          <div class="no-data">
            <mat-icon>account_tree</mat-icon>
            <p>No pipelines available</p>
            <button mat-raised-button color="primary" (click)="createPipeline()">
              Create your first pipeline
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="pipelines()" class="pipelines-table">
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
              <td mat-cell *matCellDef="let pipeline">
                <strong>{{ pipeline.name }}</strong>
                @if (pipeline.description) {
                  <div class="text-secondary text-sm">{{ pipeline.description }}</div>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let pipeline">
                <mat-chip [color]="pipeline.enabled ? 'primary' : ''">
                  {{ pipeline.enabled ? 'Enabled' : 'Disabled' }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="trigger">
              <th mat-header-cell *matHeaderCellDef>Trigger</th>
              <td mat-cell *matCellDef="let pipeline">
                @if (pipeline.triggerState) {
                  <mat-chip color="accent">
                    {{ getTriggerStateDisplay(pipeline.triggerState) }}
                  </mat-chip>
                } @else {
                  <mat-chip>Manual</mat-chip>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="steps">
              <th mat-header-cell *matHeaderCellDef>Steps</th>
              <td mat-cell *matCellDef="let pipeline">
                <div class="steps-preview">
                  <mat-icon class="steps-icon">list</mat-icon>
                  <span>{{ pipeline.steps.length }} step(s)</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let pipeline">
                <button
                  mat-icon-button
                  color="primary"
                  matTooltip="Edit"
                  (click)="editPipeline(pipeline); $event.stopPropagation()"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  matTooltip="Delete"
                  (click)="deletePipeline(pipeline); $event.stopPropagation()"
                >
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <ng-container matColumnDef="expandedDetail">
              <td mat-cell *matCellDef="let pipeline" [attr.colspan]="displayedColumns.length">
                <div class="expanded-detail">
                  <h4>Pipeline Steps</h4>
                  <div class="steps-list">
                    @for (step of pipeline.steps; track step.order) {
                      <div class="step-item">
                        <div class="step-order">{{ step.order }}</div>
                        <div class="step-content">
                          @if (step.stepType === 'TaskStep') {
                            <mat-icon class="step-icon">psychology</mat-icon>
                            <span>Task Step (ID: {{ step.taskId }})</span>
                          } @else {
                            <mat-icon class="step-icon">schedule</mat-icon>
                            <span>Wait {{ step.waitDurationSeconds }}s</span>
                          }
                        </div>
                      </div>
                    }
                  </div>
                </div>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr
              mat-row
              *matRowDef="let row; columns: displayedColumns"
              class="table-row"
              (click)="toggleExpandedRow(row)"
              [class.expanded]="expandedElement === row"
            ></tr>
            <tr
              mat-row
              *matRowDef="let row; columns: ['expandedDetail']"
              class="detail-row"
              [class.expanded]="expandedElement === row"
            ></tr>
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

    .pipelines-table {
      width: 100%;
    }

    .table-row {
      cursor: pointer;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.04);
    }

    .table-row.expanded {
      border-bottom: none;
    }

    .detail-row {
      height: 0;
      overflow: hidden;
    }

    .detail-row.expanded {
      height: auto;
    }

    .expanded-detail {
      padding: 16px;
      background-color: #f5f5f5;
      border-bottom: 1px solid #ddd;

      h4 {
        margin: 0 0 12px 0;
        font-size: 14px;
        font-weight: 500;
      }
    }

    .steps-list {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .step-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 8px;
      background-color: white;
      border-radius: 4px;
      border: 1px solid #e0e0e0;
    }

    .step-order {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 24px;
      height: 24px;
      border-radius: 50%;
      background-color: #2196f3;
      color: white;
      font-size: 12px;
      font-weight: 500;
    }

    .step-content {
      display: flex;
      align-items: center;
      gap: 8px;
      flex: 1;
    }

    .step-icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .steps-preview {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .steps-icon {
      font-size: 20px;
      width: 20px;
      height: 20px;
    }

    .text-secondary {
      color: rgba(0, 0, 0, 0.6);
    }

    .text-sm {
      font-size: 0.875rem;
    }
  `],
})
export class PipelinesListComponent implements OnInit {
  private readonly pipelineService = inject(PipelineService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  pipelines = signal<Pipeline[]>([]);
  loading = signal(true);
  selection = new SelectionModel<Pipeline>(true, []);
  displayedColumns = ['select', 'name', 'status', 'trigger', 'steps', 'actions'];
  expandedElement: Pipeline | null = null;

  ngOnInit() {
    this.loadPipelines();
  }

  loadPipelines() {
    this.loading.set(true);
    this.pipelineService.getAll().subscribe({
      next: (pipelines) => {
        this.pipelines.set(pipelines);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading pipelines:', error);
        this.loading.set(false);
      },
    });
  }

  createPipeline() {
    this.router.navigate(['/pipelines', 'new']);
  }

  editPipeline(pipeline: Pipeline) {
    this.router.navigate(['/pipelines', pipeline.id]);
  }

  deletePipeline(pipeline: Pipeline) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('pipelines.confirm_delete_single', { name: pipeline.name }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.pipelineService.delete(pipeline.id).subscribe(() => {
          this.loadPipelines();
          this.snackBar.open(
            this.translate.instant('pipelines.delete_success'),
            undefined,
            { duration: 3000 }
          );
        });
      }
    });
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.pipelines().length;
    return numSelected === numRows && numRows > 0;
  }

  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.pipelines().forEach(row => this.selection.select(row));
    }
  }

  toggleRow(row: Pipeline): void {
    this.selection.toggle(row);
  }

  async deleteSelected(): Promise<void> {
    const selectedPipelines = this.selection.selected;
    const count = selectedPipelines.length;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('pipelines.confirm_delete_multiple', { count }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    try {
      const deletePromises = selectedPipelines.map(pipeline =>
        firstValueFrom(this.pipelineService.delete(pipeline.id))
      );

      await Promise.all(deletePromises);

      this.selection.clear();
      this.loadPipelines();
      this.snackBar.open(
        this.translate.instant('pipelines.delete_multiple_success', { count }),
        undefined,
        { duration: 3000 }
      );
    } catch (error) {
      console.error('Error deleting pipelines:', error);
      this.snackBar.open(
        this.translate.instant('pipelines.delete_error'),
        undefined,
        { duration: 5000 }
      );
    }
  }

  toggleExpandedRow(pipeline: Pipeline) {
    this.expandedElement = this.expandedElement === pipeline ? null : pipeline;
  }

  getTriggerStateDisplay(triggerState: string): string {
    switch (triggerState) {
      case 'Consumed':
        return 'On Document Consumed';
      case 'Downloaded':
        return 'On Document Downloaded';
      case 'Processed':
        return 'On Document Processed';
      default:
        return triggerState;
    }
  }
}
