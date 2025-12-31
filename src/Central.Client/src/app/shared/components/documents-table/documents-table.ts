import { AfterViewInit, Component, inject, Input, input, ViewChild, viewChild } from "@angular/core";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatSort, MatSortModule } from "@angular/material/sort";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { Store } from "@ngxs/store";
import { Document as DocumentModel } from '../../models/document.model';
import { RouterLink } from "@angular/router";
import { MatIconModule } from "@angular/material/icon";
import { MatTooltipModule } from "@angular/material/tooltip";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { DatePipe } from "@angular/common";
import { CorrespondentsState, DocumentsActions, DocumentTypesState, TagsState } from "@core";
import { DocumentService } from "app/routes/documents/document.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatButtonModule } from "@angular/material/button";

@Component({
  selector: 'app-documents-table',
  standalone: true,
  templateUrl: './documents-table.html',
  imports: [
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    RouterLink,
    MatIconModule,
    MatTooltipModule,
    TranslateModule,
    DatePipe,
    MatButtonModule
  ]
})
export class DocumentsTable implements AfterViewInit{
  private readonly store = inject(Store);
  private readonly documentService = inject(DocumentService);
  private readonly translate = inject(TranslateService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly documentTypes = this.store.selectSignal(DocumentTypesState.documentTypes);
  private readonly correspondents = this.store.selectSignal(CorrespondentsState.correspondents);
  private readonly tags = this.store.selectSignal(TagsState.tags);

  @ViewChild(MatSort) sort!: MatSort;

  protected dataSource = new MatTableDataSource<{title: string, documentType: string, correspondent: string, tags: string, documentDate: string | null, updated: string | null}>([]);
  protected displayedColumns = ['title', 'documentType', 'correspondent', 'tags', 'documentDate', 'updated', 'actions'];

  @Input() set documents(value: DocumentModel[]) {
    this.dataSource.data = value.map(doc => ({
      ...doc,
      tags: this.getTagNames(doc.tagIds),
      correspondent: this.getCorrespondentName(doc.correspondentId),
      documentType: this.getDocumentTypeName(doc.documentTypeId)
    }));
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  getThumbnailUrl(id: number): string {
      return `/api/documents/${id}/thumbnail`;
    }

    getDocumentTypeName(id: number | null): string {
      if (!id) return '-';
      const docType = this.documentTypes().find(dt => dt.id === id);
      return docType?.name || '-';
    }

    getCorrespondentName(id: number | null): string {
      if (!id) return '-';
      const correspondent = this.correspondents().find(c => c.id === id);
      return correspondent?.name || '-';
    }

    getTagNames(tagIds: number[]): string {
      if (!tagIds || tagIds.length === 0) return '-';
      const tagsList = this.tags();
      const names = tagIds
        .map(id => tagsList.find(t => t.id === id)?.name)
        .filter(name => name !== undefined);
      return names.length > 0 ? names.join(', ') : '-';
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
