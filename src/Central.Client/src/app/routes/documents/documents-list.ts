import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store, Select } from '@ngxs/store';
import { PageHeader } from '@shared';
import { DocumentsState, DocumentsActions, TagsState, TagsActions } from '@core';
import { DocumentService } from './document.service';
import { Document as DocumentModel } from './document.model';
import { DocumentQuickUploadDialog } from './document-quick-upload-dialog';
import { Tag } from '../../models/tag.model';
import { Observable } from 'rxjs';
import { MtxSelectModule } from '@ng-matero/extensions/select';

@Component({
  selector: 'app-documents-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule,
    TranslateModule,
    PageHeader,
    MtxSelectModule,
  ],
  template: `
    <page-header></page-header>

    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <div class="filter-group">
            <label class="filter-label">{{ 'documents.tags' | translate }}</label>
            <mtx-select
              [(ngModel)]="selectedTagIds"
              [items]="(tags$ | async) ?? []"
              bindLabel="name"
              bindValue="id"
              [multiple]="true"
              [closeOnSelect]="false"
              placeholder="{{ 'documents.select_tags' | translate }}"
              class="tag-filter"
            ></mtx-select>
          </div>
          <div class="button-group">
            <button mat-raised-button color="primary" (click)="openQuickUploadDialog()">
              <mat-icon>upload</mat-icon>
              {{ 'documents.quick_upload' | translate }}
            </button>
          </div>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'documents.loading_documents' | translate }}</div>
        } @else if (documents().length === 0) {
          <div class="no-data">
            <mat-icon>description</mat-icon>
            <p>{{ 'documents.no_documents_found' | translate }}</p>
            <button mat-raised-button color="primary" (click)="openQuickUploadDialog()">
              {{ 'documents.upload_first_document' | translate }}
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="documents()" class="documents-table">
            <!-- Thumbnail Column -->
            <ng-container matColumnDef="thumbnail">
              <th mat-header-cell *matHeaderCellDef>{{ 'documents.preview' | translate }}</th>
              <td mat-cell *matCellDef="let document">
                @if (document.thumbnail) {
                  <img [src]="getThumbnailUrl(document.id)" alt="Thumbnail" class="thumbnail" />
                } @else {
                  <mat-icon class="file-icon">description</mat-icon>
                }
              </td>
            </ng-container>

            <!-- Title Column -->
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>{{ 'documents.title' | translate }}</th>
              <td mat-cell *matCellDef="let document">
                <a [routerLink]="['/documents', document.id]" class="document-link">
                  {{ document.title }}
                </a>
              </td>
            </ng-container>

            <!-- Document Date Column -->
            <ng-container matColumnDef="documentDate">
              <th mat-header-cell *matHeaderCellDef>{{ 'documents.document_date' | translate }}</th>
              <td mat-cell *matCellDef="let document">
                {{ document.documentDate ? (document.documentDate | date: 'short') : '-' }}
              </td>
            </ng-container>

            <!-- Updated Column -->
            <ng-container matColumnDef="updated">
              <th mat-header-cell *matHeaderCellDef>{{ 'documents.last_updated' | translate }}</th>
              <td mat-cell *matCellDef="let document">
                {{ document.updated | date: 'short' }}
              </td>
            </ng-container>

            <!-- Actions Column -->
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'documents.actions' | translate }}</th>
              <td mat-cell *matCellDef="let document">
                <button mat-icon-button [routerLink]="['/documents', document.id]" [matTooltip]="'documents.view_details' | translate">
                  <mat-icon>visibility</mat-icon>
                </button>
                <button mat-icon-button (click)="downloadDocument(document)" [matTooltip]="'documents.download' | translate">
                  <mat-icon>download</mat-icon>
                </button>
                <button mat-icon-button (click)="deleteDocument(document)" color="warn" [matTooltip]="'delete' | translate">
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
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      gap: 16px;
    }

    .filter-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
      min-width: 300px;

      .filter-label {
        font-size: 14px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.6);
      }

      .tag-filter {
        width: 100%;
      }
    }

    .button-group {
      display: flex;
      gap: 12px;
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

    .documents-table {
      width: 100%;

      .thumbnail {
        width: 50px;
        height: 50px;
        object-fit: cover;
        border-radius: 4px;
      }

      .file-icon {
        color: #999;
        font-size: 40px;
        width: 40px;
        height: 40px;
      }

      .document-link {
        color: #1976d2;
        text-decoration: none;
        font-weight: 500;

        &:hover {
          text-decoration: underline;
        }
      }

      .table-row {
        cursor: pointer;

        &:hover {
          background-color: #f5f5f5;
        }
      }
    }
  `],
})
export class DocumentsList implements OnInit {
  private readonly store = inject(Store);
  private readonly documentService = inject(DocumentService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);
  private readonly snackBar = inject(MatSnackBar);

  @Select(TagsState.tags) tags$!: Observable<Tag[]>;

  // Convert observables to signals using selectSignal
  documents = this.store.selectSignal(DocumentsState.documents);
  loading = this.store.selectSignal(DocumentsState.loading);

  selectedTagIds: number[] = [];
  displayedColumns = ['thumbnail', 'title', 'documentDate', 'updated', 'actions'];

  ngOnInit() {
    this.loadDocuments();
    this.store.dispatch(new TagsActions.Load());
  }

  loadDocuments() {
    this.store.dispatch(new DocumentsActions.Load());
  }

  getThumbnailUrl(id: number): string {
    return `/api/documents/${id}/thumbnail`;
  }

  openQuickUploadDialog() {
    const dialogRef = this.dialog.open(DocumentQuickUploadDialog, {
      width: '600px',
    });

    dialogRef.afterClosed().subscribe(result => {});
  }

  downloadDocument(doc: DocumentModel) {
    this.documentService.downloadOriginal(doc.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = doc.originalFile?.fileName || 'document';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.translate.get('documents.failed_to_download').subscribe((msg: string) => {
          this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
        });
      },
    });
  }

  deleteDocument(doc: DocumentModel) {
    this.translate.get('documents.delete_confirm', { title: doc.title }).subscribe((msg: string) => {
      if (confirm(msg)) {
        this.store.dispatch(new DocumentsActions.Delete(doc.id));
      }
    });
  }
}
