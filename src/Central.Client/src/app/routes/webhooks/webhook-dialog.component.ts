import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { Webhook, WebhookEventTypes } from '../../models/webhook.model';

@Component({
  selector: 'app-webhook-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>
      {{ (data ? 'webhooks.edit' : 'webhooks.create') | translate }}
    </h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'webhooks.name' | translate }}</mat-label>
          <input matInput formControlName="name" />
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'webhooks.description' | translate }}</mat-label>
          <textarea matInput formControlName="description" rows="3"></textarea>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'webhooks.eventType' | translate }}</mat-label>
          <mat-select formControlName="eventType" required>
            @for (eventType of eventTypes; track eventType.value) {
              <mat-option [value]="eventType.value">
                {{ eventType.label }}
              </mat-option>
            }
          </mat-select>
          @if (form.get('eventType')?.invalid && form.get('eventType')?.touched) {
            <mat-error>{{ 'webhooks.eventTypeRequired' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>{{ 'webhooks.url' | translate }}</mat-label>
          <input matInput formControlName="url" type="url" required />
          @if (form.get('url')?.hasError('required') && form.get('url')?.touched) {
            <mat-error>{{ 'webhooks.urlRequired' | translate }}</mat-error>
          }
          @if (form.get('url')?.hasError('pattern') && form.get('url')?.touched) {
            <mat-error>{{ 'webhooks.urlInvalid' | translate }}</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">{{ 'cancel' | translate }}</button>
      <button mat-raised-button color="primary" (click)="onSave()" [disabled]="form.invalid">
        {{ 'save' | translate }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    mat-dialog-content {
      padding-top: 20px;
    }
  `]
})
export class WebhookDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<WebhookDialogComponent>);

  eventTypes = WebhookEventTypes;

  form: FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Webhook | null) {
    this.form = this.fb.group({
      name: [data?.name || ''],
      description: [data?.description || ''],
      eventType: [data?.eventType || '', Validators.required],
      url: [
        data?.url || '',
        [Validators.required, Validators.pattern(/^https?:\/\/.+/)]
      ]
    });
  }

  onSave() {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
