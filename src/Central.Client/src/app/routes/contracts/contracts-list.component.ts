import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatBadgeModule } from '@angular/material/badge';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SelectionModel } from '@angular/cdk/collections';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import { ContractsState, ContractsActions, DocumentsState, DocumentsActions } from '@core';
import { Contract } from '../../models/contract.model';
import { ContractDialogComponent } from './contract-dialog.component';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-contracts-list',
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
    MatBadgeModule,
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
            {{ 'contracts.create' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'common.loading' | translate }}</div>
        } @else if (contracts().length === 0) {
          <div class="no-data">
            <mat-icon>description</mat-icon>
            <p>{{ 'contracts.noData' | translate }}</p>
            <button mat-raised-button color="primary" (click)="openDialog()">
              {{ 'contracts.createFirst' | translate }}
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="contracts()" class="contracts-table">
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
              <th mat-header-cell *matHeaderCellDef>{{ 'contracts.name' | translate }}</th>
              <td mat-cell *matCellDef="let contract">
                <strong>{{ contract.name }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="state">
              <th mat-header-cell *matHeaderCellDef>{{ 'contracts.state' | translate }}</th>
              <td mat-cell *matCellDef="let contract">
                <mat-chip [class]="'state-' + contract.state.toLowerCase()">
                  {{ 'contracts.states.' + contract.state | translate }}
                </mat-chip>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>{{ 'contracts.description' | translate }}</th>
              <td mat-cell *matCellDef="let contract">{{ contract.description || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'common.actions' | translate }}</th>
              <td mat-cell *matCellDef="let contract">
                <button
                  mat-icon-button
                  color="accent"
                  [matTooltip]="'view_documents' | translate"
                  [matBadge]="getDocumentCount(contract.id)"
                  matBadgeSize="small"
                  (click)="viewDocuments(contract.id); $event.stopPropagation()"
                >
                  <mat-icon>description</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'common.view' | translate"
                  (click)="viewContract(contract)"
                >
                  <mat-icon>visibility</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'common.edit' | translate"
                  (click)="openDialog(contract)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'common.delete' | translate"
                  (click)="deleteContract(contract)"
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
      padding: 40px 20px;
      text-align: center;
    }

    .no-data mat-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: rgba(0, 0, 0, 0.26);
      margin-bottom: 16px;
    }

    .contracts-table {
      width: 100%;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.04);
      cursor: pointer;
    }

    mat-chip {
      font-size: 12px;
    }

    mat-chip.state-draft {
      background-color: #9e9e9e;
      color: white;
    }

    mat-chip.state-active {
      background-color: #4caf50;
      color: white;
    }

    mat-chip.state-expired {
      background-color: #ff9800;
      color: white;
    }

    mat-chip.state-terminated {
      background-color: #f44336;
      color: white;
    }
  `],
})
export class ContractsListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns = ['select', 'name', 'state', 'description', 'actions'];
  selection = new SelectionModel<Contract>(true, []);

  contracts = this.store.selectSignal(ContractsState.contracts);
  loading = this.store.selectSignal(ContractsState.loading);
  documents = this.store.selectSignal(DocumentsState.documents);

  ngOnInit() {
    this.store.dispatch(new ContractsActions.Load());
    this.store.dispatch(new DocumentsActions.Load());
  }

  openDialog(contract?: Contract) {
    const dialogRef = this.dialog.open(ContractDialogComponent, {
      width: '600px',
      data: contract,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (contract) {
          this.store.dispatch(
            new ContractsActions.Update(
              contract.id,
              result.name,
              result.state,
              result.description,
              result.correspondentId,
              result.customerId,
              result.contractId
            )
          );
        } else {
          this.store.dispatch(
            new ContractsActions.Add(
              result.name,
              result.state,
              result.description,
              result.correspondentId,
              result.customerId,
              result.contractId
            )
          );
        }
      }
    });
  }

  viewContract(contract: Contract) {
    this.router.navigate(['/contracts', contract.id]);
  }

  deleteContract(contract: Contract) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('contracts.confirm_delete_single', { name: contract.name }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.store.dispatch(new ContractsActions.Delete(contract.id));
        this.snackBar.open(
          this.translate.instant('contracts.delete_success'),
          undefined,
          { duration: 3000 }
        );
      }
    });
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.contracts().length;
    return numSelected === numRows && numRows > 0;
  }

  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.contracts().forEach(row => this.selection.select(row));
    }
  }

  toggleRow(row: Contract): void {
    this.selection.toggle(row);
  }

  async deleteSelected(): Promise<void> {
    const selectedContracts = this.selection.selected;
    const count = selectedContracts.length;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('contracts.confirm_delete_multiple', { count }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    try {
      selectedContracts.forEach(contract => {
        this.store.dispatch(new ContractsActions.Delete(contract.id));
      });

      this.selection.clear();
      this.snackBar.open(
        this.translate.instant('contracts.delete_multiple_success', { count }),
        undefined,
        { duration: 3000 }
      );
    } catch (error) {
      console.error('Error deleting contracts:', error);
      this.snackBar.open(
        this.translate.instant('contracts.delete_error'),
        undefined,
        { duration: 5000 }
      );
    }
  }

  getDocumentCount(contractId: number): number {
    return this.documents().filter(doc => doc.contractId === contractId).length;
  }

  viewDocuments(contractId: number): void {
    this.router.navigate(['/documents'], { queryParams: { contractId } });
  }
}
