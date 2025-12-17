import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import { DocumentTypesState, DocumentTypesActions } from '@core';
import { DocumentType } from '../../models/document-type.model';
import { DocumentTypeDialogComponent } from './document-type-dialog.component';

@Component({
  selector: 'app-document-types-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatTableModule,
    MatTooltipModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header></page-header>
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button color="primary" (click)="openDialog()">
            <mat-icon>add</mat-icon>
            {{ 'documentTypes.create' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">Loading document types...</div>
        } @else if (documentTypes().length === 0) {
          <div class="no-data">
            <mat-icon>category</mat-icon>
            <p>No document types available</p>
            <button mat-raised-button color="primary" (click)="openDialog()">
              Create your first document type
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="documentTypes()" class="document-types-table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>{{ 'documentTypes.name' | translate }}</th>
              <td mat-cell *matCellDef="let type">
                <strong>{{ type.name }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>{{ 'documentTypes.description' | translate }}</th>
              <td mat-cell *matCellDef="let type">{{ type.description || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let type">
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'edit' | translate"
                  (click)="openDialog(type)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'delete' | translate"
                  (click)="deleteDocumentType(type)"
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

    .document-types-table {
      width: 100%;

      .table-row {
        &:hover {
          background-color: #f5f5f5;
        }
      }
    }
  `],
})
export class DocumentTypesListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  documentTypes = this.store.selectSignal(DocumentTypesState.documentTypes);
  loading = this.store.selectSignal(DocumentTypesState.loading);

  displayedColumns = ['name', 'description', 'actions'];

  ngOnInit() {
    this.store.dispatch(new DocumentTypesActions.Load());
  }

  openDialog(documentType?: DocumentType) {
    const dialogRef = this.dialog.open(DocumentTypeDialogComponent, {
      width: '500px',
      data: documentType,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (documentType) {
          this.store.dispatch(new DocumentTypesActions.Update(documentType.id, result.name, result.description));
        } else {
          this.store.dispatch(new DocumentTypesActions.Add(result.name, result.description));
        }
      }
    });
  }

  deleteDocumentType(documentType: DocumentType) {
    this.translate
      .get('documentTypes.confirmDelete')
      .subscribe((msg: string) => {
        if (confirm(msg)) {
          this.store.dispatch(new DocumentTypesActions.Delete(documentType.id));
        }
      });
  }
}
