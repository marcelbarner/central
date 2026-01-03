import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import { ProcessingService } from '../services/processing.service';
import { ProcessDefinition, DocumentState } from '../models/process.models';

@Component({
  selector: 'app-process-definitions-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    MatSlideToggleModule,
    MatChipsModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button color="primary" (click)="navigateToCreate()">
            <mat-icon>add</mat-icon>
            {{ 'processing.create' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'processing.loading' | translate }}</div>
        } @else if (processDefinitions().length === 0) {
          <div class="no-data">
            <mat-icon>autorenew</mat-icon>
            <p>{{ 'processing.no_definitions' | translate }}</p>
            <button mat-raised-button color="primary" (click)="navigateToCreate()">
              {{ 'processing.create_first' | translate }}
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="processDefinitions()" class="process-definitions-table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.name' | translate }}</th>
              <td mat-cell *matCellDef="let process">
                <strong>{{ process.name }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.description' | translate }}</th>
              <td mat-cell *matCellDef="let process">{{ process.description || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="triggerState">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.triggerState' | translate }}</th>
              <td mat-cell *matCellDef="let process">
                <mat-chip>{{ process.triggerState }}</mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="steps">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.steps' | translate }}</th>
              <td mat-cell *matCellDef="let process">
                {{ process.steps.length }}
              </td>
            </ng-container>

            <ng-container matColumnDef="enabled">
              <th mat-header-cell *matHeaderCellDef>{{ 'processing.enabled' | translate }}</th>
              <td mat-cell *matCellDef="let process">
                <mat-slide-toggle
                  [checked]="process.enabled"
                  (change)="toggleEnabled(process)"
                  color="primary"
                ></mat-slide-toggle>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let process">
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'edit' | translate"
                  (click)="navigateToEdit(process.id)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'delete' | translate"
                  (click)="confirmDelete(process)"
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

    .process-definitions-table {
      width: 100%;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.02);
      cursor: pointer;
    }
  `]
})
export class ProcessDefinitionsListComponent implements OnInit {
  private readonly processingService = inject(ProcessingService);
  private readonly router = inject(Router);
  private readonly translate = inject(TranslateService);

  processDefinitions = signal<ProcessDefinition[]>([]);
  loading = signal(false);

  displayedColumns = ['name', 'description', 'triggerState', 'steps', 'enabled', 'actions'];

  ngOnInit(): void {
    this.loadProcessDefinitions();
  }

  navigateToCreate(): void {
    this.router.navigate(['/processing-jobs/definitions/new']);
  }

  navigateToEdit(id: number): void {
    this.router.navigate(['/processing-jobs/definitions', id]);
  }

  loadProcessDefinitions(): void {
    this.loading.set(true);

    this.processingService.getProcessDefinitions().subscribe({
      next: (definitions) => {
        this.processDefinitions.set(definitions);
        this.loading.set(false);
      },
      error: (err: unknown) => {
        this.loading.set(false);
        console.error('Error loading process definitions:', err);
      }
    });
  }

  toggleEnabled(process: ProcessDefinition): void {
    const updated = {
      ...process,
      enabled: !process.enabled
    };

    this.processingService.updateProcessDefinition(process.id, updated).subscribe({
      next: () => {
        this.loadProcessDefinitions();
      },
      error: (err: unknown) => {
        console.error('Error updating process:', err);
      }
    });
  }

  confirmDelete(process: ProcessDefinition): void {
    this.translate
      .get('processing.confirmDelete', { name: process.name })
      .subscribe((msg: string) => {
        if (confirm(msg)) {
          this.processingService.deleteProcessDefinition(process.id).subscribe({
            next: () => {
              this.loadProcessDefinitions();
            },
            error: (err: unknown) => {
              console.error('Error deleting process:', err);
            }
          });
        }
      });
  }
}
