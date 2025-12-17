import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store, Select } from '@ngxs/store';
import { DocumentsActions, TagsState, TagsActions, DocumentTypesState, DocumentTypesActions, CorrespondentsState, CorrespondentsActions } from '@core';
import { DocumentService } from './document.service';
import { Tag } from '../../models/tag.model';
import { DocumentType } from '../../models/document-type.model';
import { Correspondent } from '../../models/correspondent.model';
import { Observable } from 'rxjs';
import { MtxSelectModule } from '@ng-matero/extensions/select';

@Component({
  selector: 'app-document-upload-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatSnackBarModule,
    TranslateModule,
    MtxSelectModule,
  ],
  template: `
    <h2 mat-dialog-title>{{ 'documents.upload_document_title' | translate }}</h2>
    <mat-dialog-content>
      <form #uploadForm="ngForm">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'documents.title' | translate }}</mat-label>
          <input matInput [(ngModel)]="title" name="title" required />
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'documents.document_date' | translate }}</mat-label>
          <input matInput type="datetime-local" [(ngModel)]="documentDate" name="documentDate" />
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'documents.content' | translate }}</mat-label>
          <textarea matInput [(ngModel)]="content" name="content" rows="4"></textarea>
        </mat-form-field>

        <div class="select-wrapper">
          <label class="select-label">{{ 'documents.document_type' | translate }}</label>
          <mtx-select
            [(ngModel)]="selectedDocumentTypeId"
            appendTo="body"
            name="documentType"
            [items]="(documentTypes$ | async) ?? []"
            bindLabel="name"
            bindValue="id"
            [placeholder]="'documents.select_document_type' | translate"
          ></mtx-select>
        </div>

        <div class="select-wrapper">
          <label class="select-label">{{ 'documents.correspondent' | translate }}</label>
          <mtx-select
            [(ngModel)]="selectedCorrespondentId"
            appendTo="body"
            name="correspondent"
            [items]="(correspondents$ | async) ?? []"
            bindLabel="name"
            bindValue="id"
            [placeholder]="'documents.select_correspondent' | translate"
          ></mtx-select>
        </div>

        <div class="tag-select-wrapper">
          <label class="tag-label">{{ 'documents.tags' | translate }}</label>
          <mtx-select
            [(ngModel)]="selectedTagIds"
            appendTo="body"
            name="tags"
            [items]="(tags$ | async) ?? []"
            bindLabel="name"
            bindValue="id"
            [multiple]="true"
            [closeOnSelect]="false"
            [placeholder]="'documents.select_tags' | translate"
          ></mtx-select>
        </div>

        <div class="file-upload">
          <input
            type="file"
            #fileInput
            (change)="onFileSelected($event)"
            accept=".pdf,.doc,.docx,.txt"
            style="display: none"
          />
          <button mat-raised-button type="button" (click)="fileInput.click()">
            <mat-icon>attach_file</mat-icon>
            {{ 'documents.select_file' | translate }}
          </button>
          @if (selectedFile) {
            <span class="file-name">{{ selectedFile.name }}</span>
          } @else {
            <span class="file-hint">{{ 'documents.no_file_selected' | translate }}</span>
          }
        </div>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">{{ 'documents.cancel' | translate }}</button>
      <button
        mat-raised-button
        color="primary"
        [disabled]="!canUpload() || uploading"
        (click)="upload()"
      >
        @if (uploading) {
          {{ 'documents.uploading' | translate }}
        } @else {
          {{ 'documents.upload' | translate }}
        }
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    mat-dialog-content {
      min-width: 500px;
      padding-top: 20px;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .tag-select-wrapper {
      margin-bottom: 16px;

      .tag-label {
        display: block;
        font-size: 14px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.6);
        margin-bottom: 8px;
      }
    }

    .select-wrapper {
      margin-bottom: 16px;

      .select-label {
        display: block;
        font-size: 14px;
        font-weight: 500;
        color: rgba(0, 0, 0, 0.6);
        margin-bottom: 8px;
      }
    }

    .file-upload {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 16px;

      .file-name {
        color: #1976d2;
        font-weight: 500;
      }

      .file-hint {
        color: #999;
        font-size: 14px;
      }
    }
  `],
})
export class DocumentUploadDialog {
  private readonly dialogRef = inject(MatDialogRef<DocumentUploadDialog>);
  private readonly store = inject(Store);
  private readonly documentService = inject(DocumentService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  @Select(TagsState.tags) tags$!: Observable<Tag[]>;
  @Select(DocumentTypesState.documentTypes) documentTypes$!: Observable<DocumentType[]>;
  @Select(CorrespondentsState.correspondents) correspondents$!: Observable<Correspondent[]>;

  title = '';
  documentDate: string | null = null;
  content: string | null = null;
  selectedFile: File | null = null;
  selectedDocumentTypeId: number | null = null;
  selectedCorrespondentId: number | null = null;
  selectedTagIds: number[] = [];
  uploading = false;

  constructor() {
    // Load tags, document types, and correspondents when dialog opens
    this.store.dispatch(new TagsActions.Load());
    this.store.dispatch(new DocumentTypesActions.Load());
    this.store.dispatch(new CorrespondentsActions.Load());
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      if (!this.title) {
        this.title = this.selectedFile.name.replace(/\.[^/.]+$/, '');
      }
    }
  }

  canUpload(): boolean {
    return !!this.title.trim() && !!this.selectedFile;
  }

  upload() {
    if (!this.canUpload()) return;

    this.uploading = true;
    this.store.dispatch(new DocumentsActions.Add({
      title: this.title,
      documentDate: this.documentDate,
      content: this.content,
      originalFile: this.selectedFile!,
      documentTypeId: this.selectedDocumentTypeId,
      correspondentId: this.selectedCorrespondentId,
      tagIds: this.selectedTagIds,
    }))
      .subscribe({
        next: () => {
          this.translate.get('documents.uploaded_successfully').subscribe(msg => {
            this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
          });
          this.dialogRef.close(true);
        },
        error: () => {
          this.translate.get('documents.failed_to_upload').subscribe(msg => {
            this.snackBar.open(msg, this.translate.instant('close'), { duration: 3000 });
          });
          this.uploading = false;
        },
      });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
