import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { Correspondent } from '../../models/correspondent.model';

@Component({
  selector: 'app-correspondent-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>
      {{ (data ? 'correspondents.edit' : 'correspondents.create') | translate }}
    </h2>

    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-dialog-content class="space-y-4">
        <mat-form-field class="w-full">
          <mat-label>{{ 'correspondents.name' | translate }}</mat-label>
          <input matInput formControlName="name" required maxlength="100" />
          @if (form.get('name')?.hasError('required')) {
            <mat-error>Name is required</mat-error>
          }
          @if (form.get('name')?.hasError('maxlength')) {
            <mat-error>Name must not exceed 100 characters</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'correspondents.description' | translate }}</mat-label>
          <textarea
            matInput
            formControlName="description"
            rows="3"
            maxlength="500"
          ></textarea>
          @if (form.get('description')?.hasError('maxlength')) {
            <mat-error>Description must not exceed 500 characters</mat-error>
          }
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end" class="space-x-2">
        <button mat-button type="button" (click)="onCancel()">
          {{ 'cancel' | translate }}
        </button>
        <button
          mat-raised-button
          color="primary"
          type="submit"
          [disabled]="form.invalid || form.pristine"
        >
          {{ 'save' | translate }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: `
    :host {
      display: block;
    }

    mat-dialog-content {
      min-width: 400px;
    }
  `,
})
export class CorrespondentDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<CorrespondentDialogComponent>);
  private readonly fb = inject(FormBuilder);
  readonly data = inject<Correspondent | undefined>(MAT_DIALOG_DATA);

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      name: [this.data?.name || '', [Validators.required, Validators.maxLength(100)]],
      description: [this.data?.description || '', [Validators.maxLength(500)]],
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const value = this.form.value;
      this.dialogRef.close({
        name: value.name.trim(),
        description: value.description?.trim() || undefined,
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
