import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { Store, Select } from '@ngxs/store';
import { PageHeader } from '@shared';
import { DocumentsActions, TagsState, TagsActions, DocumentTypesState, DocumentTypesActions, CorrespondentsState, CorrespondentsActions } from '@core';
import { DocumentService } from './document.service';
import { Document as DocumentModel } from './document.model';
import { Tag } from '../../models/tag.model';
import { DocumentType } from '../../models/document-type.model';
import { Correspondent } from '../../models/correspondent.model';
import { Observable } from 'rxjs';
import { MtxSelectModule } from '@ng-matero/extensions/select';

@Component({
  selector: 'app-document-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSnackBarModule,
    MatTabsModule,
    MatTooltipModule,
    PdfViewerModule,
    PageHeader,
    MtxSelectModule,
  ],
  template: `
    <page-header></page-header>

    @if (loading) {
      <div class="loading-container">
        <div class="loading">Loading document...</div>
      </div>
    } @else if (document) {
      <div class="document-container">
        <!-- Left side: Details and tabs -->
        <div class="left-panel">
          <mat-card>
            <mat-card-header>
              <mat-card-title>
                @if (editMode) {
                  <mat-form-field appearance="outline" class="title-field">
                    <input matInput [(ngModel)]="editedTitle" placeholder="Document title" />
                  </mat-form-field>
                } @else {
                  {{ document.title }}
                }
              </mat-card-title>
              <div class="header-actions">
                @if (editMode) {
                  <button mat-raised-button (click)="cancelEdit()">
                    <mat-icon>close</mat-icon>
                    Cancel
                  </button>
                  <button mat-raised-button color="primary" (click)="saveChanges()" [disabled]="saving">
                    <mat-icon>save</mat-icon>
                    {{ saving ? 'Saving...' : 'Save' }}
                  </button>
                } @else {
                  <button mat-icon-button (click)="enableEdit()" matTooltip="Edit">
                    <mat-icon>edit</mat-icon>
                  </button>
                  <button mat-icon-button (click)="downloadOriginal()" matTooltip="Download Original">
                    <mat-icon>download</mat-icon>
                  </button>
                  <button mat-icon-button (click)="downloadArchive()" matTooltip="Download Archive">
                    <mat-icon>archive</mat-icon>
                  </button>
                  <button mat-icon-button (click)="goBack()" matTooltip="Back to list">
                    <mat-icon>arrow_back</mat-icon>
                  </button>
                }
              </div>
            </mat-card-header>

            <mat-card-content>
              <mat-tab-group>
                <mat-tab label="Details">
                  <div class="tab-content">
                    <div class="detail-row">
                      <label>Document Date:</label>
                      @if (editMode) {
                        <mat-form-field appearance="outline">
                          <input matInput type="datetime-local" [(ngModel)]="editedDocumentDate" />
                        </mat-form-field>
                      } @else {
                        <span>{{ document.documentDate ? (document.documentDate | date: 'medium') : 'Not set' }}</span>
                      }
                    </div>

                    <div class="detail-row">
                      <label>Content:</label>
                      @if (editMode) {
                        <mat-form-field appearance="outline" class="full-width">
                          <textarea matInput [(ngModel)]="editedContent" rows="8"></textarea>
                        </mat-form-field>
                      } @else {
                        <p class="content">{{ document.content || 'No content' }}</p>
                      }
                    </div>

                    <div class="detail-row">
                      <label>Document Type:</label>
                      @if (editMode) {
                        <div class="select-wrapper">
                          <mtx-select
                            [(ngModel)]="editedDocumentTypeId"
                            name="documentType"
                            [items]="(documentTypes$ | async) ?? []"
                            bindLabel="name"
                            bindValue="id"
                            placeholder="Select document type"
                            appendTo="body"
                          ></mtx-select>
                        </div>
                      } @else {
                        <span>{{ getDocumentTypeName(document.documentTypeId) || 'Not set' }}</span>
                      }
                    </div>

                    <div class="detail-row">
                      <label>Correspondent:</label>
                      @if (editMode) {
                        <div class="select-wrapper">
                          <mtx-select
                            [(ngModel)]="editedCorrespondentId"
                            name="correspondent"
                            [items]="(correspondents$ | async) ?? []"
                            bindLabel="name"
                            bindValue="id"
                            placeholder="Select correspondent"
                            appendTo="body"
                          ></mtx-select>
                        </div>
                      } @else {
                        <span>{{ getCorrespondentName(document.correspondentId) || 'Not set' }}</span>
                      }
                    </div>

                    <div class="detail-row">
                      <label>Tags:</label>
                      @if (editMode) {
                        <div class="tag-edit-wrapper">
                          <mtx-select
                            [(ngModel)]="editedTagIds"
                            name="tags"
                            [items]="(tags$ | async) ?? []"
                            bindLabel="name"
                            bindValue="id"
                            [multiple]="true"
                            [closeOnSelect]="false"
                            placeholder="Select tags"
                            appendTo="body"
                          ></mtx-select>
                        </div>
                      } @else {
                        <div class="tags-display">
                          @if (document.tagIds && document.tagIds.length > 0) {
                            @for (tagId of document.tagIds; track tagId) {
                              <span class="tag-chip">{{ getTagName(tagId) }}</span>
                            }
                          } @else {
                            <span class="no-tags">No tags</span>
                          }
                        </div>
                      }
                    </div>

                    <div class="detail-row">
                      <label>Created:</label>
                      <span>{{ document.added | date: 'medium' }}</span>
                    </div>

                    <div class="detail-row">
                      <label>Last Updated:</label>
                      <span>{{ document.updated | date: 'medium' }}</span>
                    </div>

                    @if (document.addedById) {
                      <div class="detail-row">
                        <label>Created By:</label>
                        <span>User ID: {{ document.addedById }}</span>
                      </div>
                    }

                    @if (document.updatedById) {
                      <div class="detail-row">
                        <label>Updated By:</label>
                        <span>User ID: {{ document.updatedById }}</span>
                      </div>
                    }
                  </div>
                </mat-tab>

                <mat-tab label="Files">
                  <div class="tab-content">
                    @if (document.originalFile) {
                      <div class="file-card">
                        <mat-icon>description</mat-icon>
                        <div class="file-info">
                          <h4>Original File</h4>
                          <p>{{ document.originalFile.fileName }}</p>
                          <button mat-raised-button (click)="downloadOriginal()">
                            <mat-icon>download</mat-icon>
                            Download
                          </button>
                        </div>
                      </div>
                    }

                    @if (document.archiveFile) {
                      <div class="file-card">
                        <mat-icon>archive</mat-icon>
                        <div class="file-info">
                          <h4>Archive File</h4>
                          <p>{{ document.archiveFile.fileName }}</p>
                          <button mat-raised-button (click)="downloadArchive()">
                            <mat-icon>download</mat-icon>
                            Download
                          </button>
                        </div>
                      </div>
                    }
                  </div>
                </mat-tab>

                @if (document.thumbnail) {
                  <mat-tab label="Thumbnail">
                    <div class="tab-content preview">
                      <img [src]="getThumbnailUrl()" alt="Document preview" class="preview-image" />
                    </div>
                  </mat-tab>
                }
              </mat-tab-group>
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Right side: PDF Preview -->
        <div class="right-panel">
          <mat-card class="pdf-card">
            <mat-card-header>
              <mat-card-title>Archive Preview</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (document.archiveFile) {
                <div class="pdf-container">
                  <pdf-viewer
                    [src]="getArchiveUrl()"
                    [render-text]="true"
                    [original-size]="false"
                    [fit-to-page]="true"
                    [show-all]="true"
                    [autoresize]="true"
                    class="pdf-viewer"
                  ></pdf-viewer>
                </div>
              } @else {
                <div class="no-preview">
                  <mat-icon>picture_as_pdf</mat-icon>
                  <p>No archive file available</p>
                </div>
              }
            </mat-card-content>
          </mat-card>
        </div>
      </div>
    } @else {
      <div class="loading-container">
        <div class="error">Document not found</div>
      </div>
    }
  `,
  styles: [`
    .loading-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 400px;
    }

    .loading,
    .error {
      text-align: center;
      padding: 40px;
      color: #666;
      font-size: 16px;
    }

    .document-container {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      height: calc(100vh - 200px);
      min-height: 600px;
    }

    .left-panel,
    .right-panel {
      height: 100%;
      overflow: hidden;
    }

    .left-panel mat-card,
    .right-panel mat-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .left-panel mat-card-content {
      flex: 1;
      overflow-y: auto;
    }

    .pdf-card {
      mat-card-content {
        flex: 1;
        overflow: hidden;
        padding: 0 !important;
      }
    }

    .pdf-container {
      height: 100%;
      overflow-y: auto;
      background-color: #525659;
    }

    .pdf-viewer {
      width: 100%;
      height: 100%;
    }

    .no-preview {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100%;
      color: #999;

      mat-icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        margin-bottom: 16px;
      }

      p {
        margin: 0;
        font-size: 16px;
      }
    }

    mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;

      mat-card-title {
        font-size: 24px;
        font-weight: 500;
        margin: 0;
        flex: 1;
      }

      .title-field {
        width: 100%;
        margin: 0;
      }

      .header-actions {
        display: flex;
        gap: 8px;
        align-items: center;
      }
    }

    .tab-content {
      padding: 24px 0;

      &.preview {
        text-align: center;
      }
    }

    .detail-row {
      display: grid;
      grid-template-columns: 150px 1fr;
      gap: 16px;
      margin-bottom: 16px;
      align-items: start;

      label {
        font-weight: 500;
        color: #666;
        padding-top: 8px;
      }

      span {
        padding-top: 8px;
      }

      .content {
        margin: 0;
        white-space: pre-wrap;
        line-height: 1.6;
      }

      mat-form-field {
        width: 100%;
      }
    }

    .full-width {
      width: 100%;
    }

    .select-wrapper,
    .tag-edit-wrapper {
      width: 100%;
    }

    .tags-display {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      padding-top: 8px;
    }

    .tag-chip {
      display: inline-block;
      padding: 4px 12px;
      background-color: #e3f2fd;
      color: #1976d2;
      border-radius: 16px;
      font-size: 14px;
      font-weight: 500;
    }

    .no-tags {
      color: #999;
      font-style: italic;
    }

    .file-card {
      display: flex;
      gap: 16px;
      padding: 16px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      margin-bottom: 16px;
      align-items: center;

      mat-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
        color: #1976d2;
      }

      .file-info {
        flex: 1;

        h4 {
          margin: 0 0 4px;
          font-size: 16px;
          font-weight: 500;
        }

        p {
          margin: 0 0 12px;
          color: #666;
          font-size: 14px;
        }
      }
    }

    .preview-image {
      max-width: 100%;
      max-height: 600px;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }
  `],
})
export class DocumentDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly documentService = inject(DocumentService);
  private readonly store = inject(Store);
  private readonly snackBar = inject(MatSnackBar);

  @Select(TagsState.tags) tags$!: Observable<Tag[]>;
  @Select(DocumentTypesState.documentTypes) documentTypes$!: Observable<DocumentType[]>;
  @Select(CorrespondentsState.correspondents) correspondents$!: Observable<Correspondent[]>;

  document: DocumentModel | null = null;
  loading = true;
  editMode = false;
  saving = false;

  editedTitle = '';
  editedDocumentDate: string | null = null;
  editedContent: string | null = null;
  editedDocumentTypeId: number | null = null;
  editedCorrespondentId: number | null = null;
  editedTagIds: number[] = [];

  private tagsMap: Map<number, string> = new Map();
  private documentTypesMap: Map<number, string> = new Map();
  private correspondentsMap: Map<number, string> = new Map();

  ngOnInit() {
    // Load tags, document types, and correspondents
    this.store.dispatch(new TagsActions.Load());
    this.store.dispatch(new DocumentTypesActions.Load());
    this.store.dispatch(new CorrespondentsActions.Load());
    
    this.tags$.subscribe(tags => {
      this.tagsMap = new Map(tags.map(t => [t.id, t.name]));
    });
    
    this.documentTypes$.subscribe(types => {
      this.documentTypesMap = new Map(types.map(t => [t.id, t.name]));
    });
    
    this.correspondents$.subscribe(correspondents => {
      this.correspondentsMap = new Map(correspondents.map(c => [c.id, c.name]));
    });

    const id = this.route.snapshot.params['id'];
    if (id) {
      this.loadDocument(+id);
    }
  }

  loadDocument(id: number) {
    this.loading = true;
    this.documentService.getById(id).subscribe({
      next: doc => {
        this.document = doc;
        this.loading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load document', 'Close', { duration: 3000 });
        this.loading = false;
      },
    });
  }

  enableEdit() {
    if (!this.document) return;
    this.editMode = true;
    this.editedTitle = this.document.title;
    this.editedDocumentDate = this.document.documentDate;
    this.editedContent = this.document.content;
    this.editedDocumentTypeId = this.document.documentTypeId;
    this.editedCorrespondentId = this.document.correspondentId;
    this.editedTagIds = [...(this.document.tagIds || [])];
  }

  cancelEdit() {
    this.editMode = false;
  }

  saveChanges() {
    if (!this.document || !this.editedTitle.trim()) return;

    this.saving = true;
    this.store.dispatch(new DocumentsActions.Update({
      id: this.document.id,
      title: this.editedTitle,
      documentDate: this.editedDocumentDate,
      content: this.editedContent,
      documentTypeId: this.editedDocumentTypeId,
      correspondentId: this.editedCorrespondentId,
      tagIds: this.editedTagIds,
    })).subscribe({
      next: () => {
        // Reload document to get updated data from server
        this.loadDocument(this.document!.id);
        this.editMode = false;
        this.saving = false;
      },
      error: () => {
        this.saving = false;
      },
    });
  }

  getTagName(tagId: number): string {
    return this.tagsMap.get(tagId) || `Tag ${tagId}`;
  }

  getDocumentTypeName(documentTypeId: number | null): string | null {
    if (!documentTypeId) return null;
    return this.documentTypesMap.get(documentTypeId) || null;
  }

  getCorrespondentName(correspondentId: number | null): string | null {
    if (!correspondentId) return null;
    return this.correspondentsMap.get(correspondentId) || null;
  }

  getThumbnailUrl(): string {
    return this.document ? this.documentService.getThumbnail(this.document.id) : '';
  }

  getArchiveUrl(): string {
    return this.document ? this.documentService.getArchiveUrl(this.document.id) : '';
  }

  downloadOriginal() {
    if (!this.document) return;
    this.documentService.downloadOriginal(this.document.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = this.document!.originalFile?.fileName || 'document';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.snackBar.open('Failed to download file', 'Close', { duration: 3000 });
      },
    });
  }

  downloadArchive() {
    if (!this.document) return;
    this.documentService.downloadArchive(this.document.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = this.document!.archiveFile?.fileName || 'archive';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.snackBar.open('Failed to download archive', 'Close', { duration: 3000 });
      },
    });
  }

  goBack() {
    this.router.navigate(['/documents']);
  }
}
