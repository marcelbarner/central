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
import { WebhooksState, WebhooksActions } from '@core';
import { Webhook } from '../../models/webhook.model';
import { WebhookDialogComponent } from './webhook-dialog.component';

@Component({
  selector: 'app-webhooks-list',
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
            {{ 'webhooks.create' | translate }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">Loading webhooks...</div>
        } @else if (webhooks().length === 0) {
          <div class="no-data">
            <mat-icon>webhook</mat-icon>
            <p>No webhooks available</p>
            <button mat-raised-button color="primary" (click)="openDialog()">
              Create your first webhook
            </button>
          </div>
        } @else {
          <table mat-table [dataSource]="webhooks()" class="webhooks-table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>{{ 'webhooks.name' | translate }}</th>
              <td mat-cell *matCellDef="let webhook">
                <strong>{{ webhook.name || '-' }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="eventType">
              <th mat-header-cell *matHeaderCellDef>{{ 'webhooks.eventType' | translate }}</th>
              <td mat-cell *matCellDef="let webhook">
                {{ webhook.eventType }}
              </td>
            </ng-container>

            <ng-container matColumnDef="url">
              <th mat-header-cell *matHeaderCellDef>{{ 'webhooks.url' | translate }}</th>
              <td mat-cell *matCellDef="let webhook">{{ webhook.url }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>{{ 'actions' | translate }}</th>
              <td mat-cell *matCellDef="let webhook">
                <button
                  mat-icon-button
                  color="primary"
                  [matTooltip]="'edit' | translate"
                  (click)="openDialog(webhook)"
                >
                  <mat-icon>edit</mat-icon>
                </button>
                <button
                  mat-icon-button
                  color="warn"
                  [matTooltip]="'delete' | translate"
                  (click)="deleteWebhook(webhook)"
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

    .loading, .no-data {
      text-align: center;
      padding: 48px 16px;
      color: rgba(0, 0, 0, 0.54);
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

    .webhooks-table {
      width: 100%;
    }

    .table-row:hover {
      background-color: rgba(0, 0, 0, 0.02);
      cursor: pointer;
    }
  `]
})
export class WebhooksListComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  webhooks = this.store.selectSignal(WebhooksState.webhooks);
  loading = this.store.selectSignal(WebhooksState.loading);

  displayedColumns = ['name', 'eventType', 'url', 'actions'];

  ngOnInit() {
    this.store.dispatch(new WebhooksActions.Load());
  }

  openDialog(webhook?: Webhook) {
    const dialogRef = this.dialog.open(WebhookDialogComponent, {
      width: '500px',
      data: webhook
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (webhook) {
          this.store.dispatch(new WebhooksActions.Update(webhook.id, result.eventType, result.url, result.name, result.description));
        } else {
          this.store.dispatch(new WebhooksActions.Add(result.eventType, result.url, result.name, result.description));
        }
      }
    });
  }

  deleteWebhook(webhook: Webhook) {
    if (confirm(this.translate.instant('webhooks.confirmDelete'))) {
      this.store.dispatch(new WebhooksActions.Delete(webhook.id));
    }
  }
}
