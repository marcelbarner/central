import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatBadgeModule } from '@angular/material/badge';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import { CorrespondentsState, CorrespondentsActions, DocumentsState, DocumentsActions } from '@core';
import { Correspondent } from '../../models/correspondent.model';
import { CorrespondentDialogComponent } from './correspondent-dialog.component';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-correspondents-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
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
          <button mat-raised-button color="primary" (click)="openDialog()">
            <mat-icon>add</mat-icon>
            {{ 'correspondents.create' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">Loading correspondents...</div>
        } @else if (correspondents().length === 0) {
          <div class="no-data">
            <mat-icon>contacts</mat-icon>
            <p>No correspondents available</p>
            <button mat-raised-button color="primary" (click)="openDialog()">
              Create your first correspondent
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="correspondents()" class="correspondents-table">
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
              <th mat-header-cell *matHeaderCellDef>{{ 'correspondents.name' | translate }}</th>
              <td mat-cell *matCellDef="let correspondent">
                <strong>{{ correspondent.name }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>{{ 'correspondents.description' | translate }}</th>
              <td mat-cell *matCellDef="let correspondent">{{ correspondent.description || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let correspondent">
                <button
                  mat-button
                  color="accent"
                  [matTooltip]="'view_documents' | translate"
                  (click)="viewDocuments(correspondent.id); $event.stopPropagation()"
                >
                  <mat-icon>description</mat-icon>
                  {{ getDocumentCount(correspondent.id) }}
                </button>
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'edit' | translate"
                  (click)="openDialog(correspondent)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'delete' | translate"
                  (click)="deleteCorrespondent(correspondent)"
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

    .correspondents-table {
      width: 100%;

      .table-row {
        &:hover {
          background-color: #f5f5f5;
        }
      }
    }
  `],
})
export class CorrespondentsListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  correspondents = this.store.selectSignal(CorrespondentsState.correspondents);
  loading = this.store.selectSignal(CorrespondentsState.loading);
  documents = this.store.selectSignal(DocumentsState.documents);

  selection = new SelectionModel<Correspondent>(true, []);
  displayedColumns = ['select', 'name', 'description', 'actions'];

  ngOnInit() {
    this.store.dispatch(new CorrespondentsActions.Load());
    this.store.dispatch(new DocumentsActions.Load());
  }

  openDialog(correspondent?: Correspondent) {
    const dialogRef = this.dialog.open(CorrespondentDialogComponent, {
      width: '500px',
      data: correspondent,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (correspondent) {
          this.store.dispatch(new CorrespondentsActions.Update(correspondent.id, result.name, result.description));
        } else {
          this.store.dispatch(new CorrespondentsActions.Add(result.name, result.description));
        }
      }
    });
  }

  deleteCorrespondent(correspondent: Correspondent) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('correspondents.confirm_delete_single', { name: correspondent.name }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.store.dispatch(new CorrespondentsActions.Delete(correspondent.id));
        this.snackBar.open(
          this.translate.instant('correspondents.delete_success'),
          undefined,
          { duration: 3000 }
        );
      }
    });
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.correspondents().length;
    return numSelected === numRows && numRows > 0;
  }

  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.correspondents().forEach(row => this.selection.select(row));
    }
  }

  toggleRow(row: Correspondent): void {
    this.selection.toggle(row);
  }

  async deleteSelected(): Promise<void> {
    const selectedCorrespondents = this.selection.selected;
    const count = selectedCorrespondents.length;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('correspondents.confirm_delete_multiple', { count }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    try {
      selectedCorrespondents.forEach(correspondent => {
        this.store.dispatch(new CorrespondentsActions.Delete(correspondent.id));
      });

      this.selection.clear();
      this.snackBar.open(
        this.translate.instant('correspondents.delete_multiple_success', { count }),
        undefined,
        { duration: 3000 }
      );
    } catch (error) {
      console.error('Error deleting correspondents:', error);
      this.snackBar.open(
        this.translate.instant('correspondents.delete_error'),
        undefined,
        { duration: 5000 }
      );
    }
  }

  getDocumentCount(correspondentId: number): number {
    return this.documents().filter(doc => doc.correspondentId === correspondentId).length;
  }

  viewDocuments(correspondentId: number): void {
    this.router.navigate(['/documents'], { queryParams: { correspondentId } });
  }
}
