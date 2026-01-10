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
import { TagsState, TagsActions, DocumentsState, DocumentsActions } from '@core';
import { Tag } from '../../models/tag.model';
import { TagDialogComponent } from './tag-dialog.component';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-tags-list',
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
          <div class="button-group">
            <button mat-raised-button color="primary" (click)="openDialog()">
              <mat-icon>add</mat-icon>
              {{ 'tags.create' | translate }}
            </button>
            <button mat-raised-button color="accent" (click)="importFromCsv()">
              <mat-icon>upload</mat-icon>
              {{ 'tags.import_csv' | translate }}
            </button>
          </div>
        </div>

        @if (loading()) {
          <div class="loading">{{ 'tags.loading_tags' | translate }}</div>
        } @else if (tags().length === 0) {
          <div class="no-data">
            <mat-icon>label</mat-icon>
            <p>{{ 'tags.no_tags' | translate }}</p>
            <button mat-raised-button color="primary" (click)="openDialog()">
              {{ 'tags.create_first_tag' | translate }}
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="tags()" class="tags-table">
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
              <th mat-header-cell *matHeaderCellDef>{{ 'tags.name' | translate }}</th>
              <td mat-cell *matCellDef="let tag">
                <strong>{{ tag.name }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>{{ 'tags.description' | translate }}</th>
              <td mat-cell *matCellDef="let tag">{{ tag.description || '-' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let tag">
                <button
                  mat-button
                  color="accent"
                  [matTooltip]="'view_documents' | translate"
                  (click)="viewDocuments(tag.id); $event.stopPropagation()"
                >
                  <mat-icon>description</mat-icon>
                  {{ getDocumentCount(tag.id) }}
                </button>
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'edit' | translate"
                  (click)="openDialog(tag)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'delete' | translate"
                  (click)="deleteTag(tag)"
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
      margin-bottom: 20px;
      gap: 16px;

      .button-group {
        display: flex;
        gap: 12px;
      }
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

    .tags-table {
      width: 100%;

      .table-row {
        &:hover {
          background-color: #f5f5f5;
        }
      }
    }
  `],
})
export class TagsListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  tags = this.store.selectSignal(TagsState.tags);
  loading = this.store.selectSignal(TagsState.loading);
  documents = this.store.selectSignal(DocumentsState.documents);

  selection = new SelectionModel<Tag>(true, []);
  displayedColumns = ['select', 'name', 'description', 'actions'];

  ngOnInit() {
    this.store.dispatch(new TagsActions.Load());
    this.store.dispatch(new DocumentsActions.Load());
  }

  openDialog(tag?: Tag) {
    const dialogRef = this.dialog.open(TagDialogComponent, {
      width: '500px',
      data: tag,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (tag) {
          this.store.dispatch(new TagsActions.Update({ id: tag.id, ...result }));
        } else {
          this.store.dispatch(new TagsActions.Add(result));
        }
      }
    });
  }

  deleteTag(tag: Tag) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('tags.confirm_delete_single', { name: tag.name }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.store.dispatch(new TagsActions.Delete(tag.id));
        this.snackBar.open(
          this.translate.instant('tags.delete_success'),
          undefined,
          { duration: 3000 }
        );
      }
    });
  }

  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.tags().length;
    return numSelected === numRows && numRows > 0;
  }

  toggleAllRows(): void {
    if (this.isAllSelected()) {
      this.selection.clear();
    } else {
      this.tags().forEach(row => this.selection.select(row));
    }
  }

  toggleRow(row: Tag): void {
    this.selection.toggle(row);
  }

  async deleteSelected(): Promise<void> {
    const selectedTags = this.selection.selected;
    const count = selectedTags.length;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'confirm_delete',
        message: this.translate.instant('tags.confirm_delete_multiple', { count }),
        confirmText: 'delete',
        cancelText: 'cancel',
        confirmColor: 'warn'
      } as ConfirmDialogData
    });

    const confirmed = await firstValueFrom(dialogRef.afterClosed());
    if (!confirmed) return;

    try {
      selectedTags.forEach(tag => {
        this.store.dispatch(new TagsActions.Delete(tag.id));
      });

      this.selection.clear();
      this.snackBar.open(
        this.translate.instant('tags.delete_multiple_success', { count }),
        undefined,
        { duration: 3000 }
      );
    } catch (error) {
      console.error('Error deleting tags:', error);
      this.snackBar.open(
        this.translate.instant('tags.delete_error'),
        undefined,
        { duration: 5000 }
      );
    }
  }

  importFromCsv() {
    // Create file input element
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = '.csv';
    input.onchange = (e: any) => {
      const file = e.target?.files?.[0];
      if (file) {
        this.processCsvFile(file);
      }
    };
    input.click();
  }

  private processCsvFile(file: File) {
    const reader = new FileReader();
    reader.onload = (e: any) => {
      const text = e.target.result;
      const lines = text.split('\n');

      // Skip header row and process data
      for (let i = 1; i < lines.length; i++) {
        const line = lines[i].trim();
        if (line) {
          const [name, description] = line.split(',').map((s: string) => s.trim());
          if (name) {
            this.store.dispatch(
              new TagsActions.Add({
                name,
                description: description || undefined,
              })
            );
          }
        }
      }
    };
    reader.readAsText(file);
  }

  getDocumentCount(tagId: number): number {
    return this.documents().filter(doc => doc.tagIds.includes(tagId)).length;
  }

  viewDocuments(tagId: number): void {
    this.router.navigate(['/documents'], { queryParams: { tagId } });
  }
}
