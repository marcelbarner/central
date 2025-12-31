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
import { CorrespondentsState, CorrespondentsActions } from '@core';
import { Correspondent } from '../../models/correspondent.model';
import { CorrespondentDialogComponent } from './correspondent-dialog.component';

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
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
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

  correspondents = this.store.selectSignal(CorrespondentsState.correspondents);
  loading = this.store.selectSignal(CorrespondentsState.loading);

  displayedColumns = ['name', 'description', 'actions'];

  ngOnInit() {
    this.store.dispatch(new CorrespondentsActions.Load());
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
    this.translate
      .get('correspondents.confirmDelete')
      .subscribe((msg: string) => {
        if (confirm(msg)) {
          this.store.dispatch(new CorrespondentsActions.Delete(correspondent.id));
        }
      });
  }
}
