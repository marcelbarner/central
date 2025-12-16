import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { forkJoin } from 'rxjs';
import { DocumentsActions } from '@core';
import { DocumentService } from './document.service';

@Component({
  selector: 'app-document-quick-upload-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>{{ 'documents.quick_upload_title' | translate }}</h2>
    <mat-dialog-content>
      <div class="upload-area" (click)="fileInput.click()" (dragover)="onDragOver($event)" (dragleave)="onDragLeave($event)" (drop)="onDrop($event)">
        <input
          type="file"
          #fileInput
          (change)="onFileSelected($event)"
          accept=".pdf,.doc,.docx,.txt"
          multiple
          style="display: none"
        />
        @if (selectedFiles.length > 0) {
          <mat-icon class="file-icon success">check_circle</mat-icon>
          @if (selectedFiles.length === 1) {
            <p class="file-name">{{ selectedFiles[0].name }}</p>
            <p class="file-size">{{ formatFileSize(selectedFiles[0].size) }}</p>
          } @else {
            <p class="file-name">{{ 'documents.files_selected' | translate: {count: selectedFiles.length} }}</p>
            <p class="file-size">{{ getTotalSize() }}</p>
          }
        } @else {
          <mat-icon class="upload-icon">cloud_upload</mat-icon>
          <p class="upload-hint">{{ 'documents.quick_upload_hint' | translate }}</p>
          <p class="file-hint">{{ 'documents.quick_upload_file_hint' | translate }}</p>
        }
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">{{ 'documents.cancel' | translate }}</button>
      <button
        mat-raised-button
        color="primary"
        [disabled]="selectedFiles.length === 0 || uploading"
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
      min-height: 250px;
      padding: 20px;
    }

    .upload-area {
      border: 2px dashed #ccc;
      border-radius: 8px;
      padding: 40px;
      text-align: center;
      cursor: pointer;
      transition: all 0.3s ease;
      background-color: #fafafa;

      &:hover {
        border-color: #1976d2;
        background-color: #f0f7ff;
      }

      &.drag-over {
        border-color: #1976d2;
        background-color: #e3f2fd;
      }

      .upload-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        color: #999;
        margin: 0 auto 16px;
      }

      .file-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        margin: 0 auto 16px;

        &.success {
          color: #4caf50;
        }
      }

      .upload-hint {
        font-size: 16px;
        color: #333;
        margin: 0 0 8px;
      }

      .file-hint {
        font-size: 14px;
        color: #999;
        margin: 0;
      }

      .file-name {
        font-size: 16px;
        font-weight: 500;
        color: #1976d2;
        margin: 0 0 8px;
        word-break: break-word;
      }

      .file-size {
        font-size: 14px;
        color: #666;
        margin: 0;
      }
    }
  `],
})
export class DocumentQuickUploadDialog {
  private readonly dialogRef = inject(MatDialogRef<DocumentQuickUploadDialog>);
  private readonly store = inject(Store);
  private readonly documentService = inject(DocumentService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  selectedFiles: File[] = [];
  uploading = false;

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const target = event.currentTarget as HTMLElement;
    target.classList.add('drag-over');
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const target = event.currentTarget as HTMLElement;
    target.classList.remove('drag-over');
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    const target = event.currentTarget as HTMLElement;
    target.classList.remove('drag-over');

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.selectedFiles = Array.from(event.dataTransfer.files);
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
  }

  getTotalSize(): string {
    const total = this.selectedFiles.reduce((sum, file) => sum + file.size, 0);
    return this.formatFileSize(total);
  }

  upload() {
    if (this.selectedFiles.length === 0) return;

    this.uploading = true;

    const uploadObservables = this.selectedFiles.map(file => {
      const title = file.name.replace(/\.[^/.]+$/, '');
      return this.store.dispatch(new DocumentsActions.Add({
        title: title,
        documentDate: null,
        content: null,
        originalFile: file,
      }));
    });

    forkJoin(uploadObservables).subscribe({
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
