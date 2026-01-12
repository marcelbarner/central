import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import {
  DocumentsState,
  DocumentsActions,
  TagsActions,
  DocumentTypesActions,
  CorrespondentsActions,
} from '@core';
import { Document as DocumentModel } from '../../shared/models/document.model';
import { DocumentQuickUploadDialog } from './document-quick-upload-dialog';
import { ExecuteTaskDialogComponent } from './execute-task-dialog.component';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { BehaviorSubject, combineLatest, map, Subscription } from 'rxjs';
import { DocumentsTable } from '@shared/components/documents-table/documents-table';
import { TagsSelect } from '@shared/components/tags-select/tags-select';
import { CorrespondentsSelect } from '@shared/components/correspondents-select/correspondents-select';
import { DocumentTypesSelect } from '@shared/components/document-types-select/document-types-select';

@Component({
  selector: 'app-documents-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatCardModule,
    MatDialogModule,
    MatSnackBarModule,
    MatSortModule,
    MatTooltipModule,
    TranslateModule,
    PageHeader,
    MtxSelectModule,
    DocumentsTable,
    TagsSelect,
    CorrespondentsSelect,
    DocumentTypesSelect
  ],
  template: `
    <page-header />

    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <div class="filter-group">
            <app-tags-select (selectedTagsChange)="filterByTags($event)" />
            <app-correspondents-select (selectedCorrespondentsChange)="filterByCorrespondents($event)" />
            <app-document-types-select (selectedDocumentTypesChange)="filterByDocumentTypes($event)" />
          </div>
          <div class="button-group">
            @if (selectedDocuments.length > 0) {
              <button mat-raised-button color="accent" (click)="executeTaskOnSelected()">
                <mat-icon>play_arrow</mat-icon>
                {{ 'documents.execute_task' | translate }} ({{ selectedDocuments.length }})
              </button>
            }
            <button mat-raised-button color="primary" (click)="openQuickUploadDialog()">
              <mat-icon>upload</mat-icon>
              {{ 'documents.quick_upload' | translate }}
            </button>
          </div>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'documents.loading_documents' | translate }}</div>
        } @else if (filteredDocuments.length === 0) {
          <div class="no-data">
            <mat-icon>description</mat-icon>
            <p>{{ 'documents.no_documents_found' | translate }}</p>
            <button mat-raised-button color="primary" (click)="openQuickUploadDialog()">
              {{ 'documents.upload_first_document' | translate }}
            </button>
          </div>
        } @else {
          <app-documents-table
            [documents]="filteredDocuments"
            (selectionChange)="onSelectionChange($event)"
          />
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .header-actions {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 20px;
        gap: 16px;
      }

      .filter-group {
        display: flex;
        flex-direction: row;
        flex-grow: 1;
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
    `,
  ],
})
export class DocumentsList implements OnInit, AfterViewInit, OnDestroy {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);
  private readonly route = inject(ActivatedRoute);
  private readonly sub = new Subscription();
  private readonly selectedTagIds = new BehaviorSubject<number[]>([]);
  private readonly selectedCorrespondentIds = new BehaviorSubject<number[]>([]);
  private readonly selectedDocumentTypeIds = new BehaviorSubject<number[]>([]);
  private readonly selectedContractIds = new BehaviorSubject<number[]>([]);

  @ViewChild(MatSort, { read: MatSort }) sort?: MatSort;

  // Convert observables to signals using selectSignal
  documents = this.store.select(DocumentsState.documents);
  loading = this.store.selectSignal(DocumentsState.loading);
  dataSource = new MatTableDataSource<DocumentModel>([]);
  filteredDocuments: DocumentModel[] = [];
  selectedDocuments: DocumentModel[] = [];

  ngOnInit() {
    this.loadDocuments();
    this.store.dispatch(new TagsActions.Load());
    this.store.dispatch(new DocumentTypesActions.Load());
    this.store.dispatch(new CorrespondentsActions.Load());

    // Handle query parameters for filtering
    this.route.queryParams.subscribe(params => {
      if (params['tagId']) {
        const tagId = parseInt(params['tagId'], 10);
        if (!isNaN(tagId)) {
          this.selectedTagIds.next([tagId]);
        }
      }
      if (params['correspondentId']) {
        const correspondentId = parseInt(params['correspondentId'], 10);
        if (!isNaN(correspondentId)) {
          this.selectedCorrespondentIds.next([correspondentId]);
        }
      }
      if (params['documentTypeId']) {
        const documentTypeId = parseInt(params['documentTypeId'], 10);
        if (!isNaN(documentTypeId)) {
          this.selectedDocumentTypeIds.next([documentTypeId]);
        }
      }
      if (params['contractId']) {
        const contractId = parseInt(params['contractId'], 10);
        if (!isNaN(contractId)) {
          // Filter by contract - need to add this to the filtering logic
          this.filterByContract(contractId);
        }
      }
    });
    this.sub.add(
      combineLatest({
        documents: this.documents,
        selectedTagIds: this.selectedTagIds.asObservable(),
        selectedCorrespondentIds: this.selectedCorrespondentIds.asObservable(),
        selectedDocumentTypeIds: this.selectedDocumentTypeIds.asObservable(),
        selectedContractIds: this.selectedContractIds.asObservable(),
      })
        .pipe(
          map(({ documents, selectedTagIds, selectedCorrespondentIds, selectedDocumentTypeIds, selectedContractIds }) => {
            if (selectedTagIds.length === 0 && selectedCorrespondentIds.length === 0 && selectedDocumentTypeIds.length === 0 && selectedContractIds.length === 0) {
              return documents;
            }
            let result = documents;
            if (selectedTagIds.length != 0) {
              result = result.filter((doc) => {
                return selectedTagIds.some((tagId) => doc.tagIds.includes(tagId));
              });
            }

            if (selectedCorrespondentIds.length != 0) {
              result = result.filter((doc) => {
                return selectedCorrespondentIds.includes(doc.correspondentId!);
              });
            }

            if (selectedDocumentTypeIds.length != 0) {
              result = result.filter((doc) => {
                return selectedDocumentTypeIds.includes(doc.documentTypeId!);
              });
            }

            if (selectedContractIds.length != 0) {
              result = result.filter((doc) => {
                return selectedContractIds.includes(doc.contractId!);
              });
            }

            return result;
          }),
        )
        .subscribe((filtered) => {
          this.filteredDocuments = filtered;
        }),
    );
  }

  ngAfterViewInit() {
    //if(this.sort) {
    this.dataSource.sort = this.sort;
    //}
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  loadDocuments() {
    this.store.dispatch(new DocumentsActions.Load());
  }

  openQuickUploadDialog() {
    const dialogRef = this.dialog.open(DocumentQuickUploadDialog, {
      width: '600px',
    });

    dialogRef.afterClosed().subscribe(() => {});
  }

  filterByTags(tagIds: number[]) {
    this.selectedTagIds.next(tagIds);
  }

  filterByCorrespondents(correspondentId: number) {
    this.selectedCorrespondentIds.next([correspondentId]);
  }
  filterByDocumentTypes(documentTypeId: number) {
    this.selectedDocumentTypeIds.next([documentTypeId]);
  }

  filterByContract(contractId: number) {
    this.selectedContractIds.next([contractId]);
  }

  onSelectionChange(documents: DocumentModel[]) {
    this.selectedDocuments = documents;
  }

  executeTaskOnSelected() {
    if (this.selectedDocuments.length === 0) {
      return;
    }

    const dialogRef = this.dialog.open(ExecuteTaskDialogComponent, {
      width: '500px',
      data: {
        documentIds: this.selectedDocuments.map(d => d.id),
        documentCount: this.selectedDocuments.length
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const { successCount, failedCount, totalCount } = result;

        if (failedCount === 0) {
          this.snackBar.open(
            this.translate.instant('documents.task_execution_success', { count: successCount }),
            this.translate.instant('close'),
            { duration: 3000 }
          );
        } else if (successCount === 0) {
          this.snackBar.open(
            this.translate.instant('documents.task_execution_failed', { count: failedCount }),
            this.translate.instant('close'),
            { duration: 5000, panelClass: ['error-snackbar'] }
          );
        } else {
          this.snackBar.open(
            this.translate.instant('documents.task_execution_partial', {
              success: successCount,
              failed: failedCount,
              total: totalCount
            }),
            this.translate.instant('close'),
            { duration: 5000 }
          );
        }

        // Clear selection after execution
        this.selectedDocuments = [];
      }
    });
  }
}
