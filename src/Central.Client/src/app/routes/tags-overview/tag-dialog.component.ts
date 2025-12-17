import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { Tag } from '../../models/tag.model';

@Component({
  selector: 'app-tag-dialog',
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
      {{ (data ? 'tags.edit' : 'tags.create') | translate }}
    </h2>

    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-dialog-content class="space-y-4">
        <mat-form-field class="w-full">
          <mat-label>{{ 'tags.name' | translate }}</mat-label>
          <input matInput formControlName="name" required maxlength="100" />
          @if (form.get('name')?.hasError('required')) {
            <mat-error>{{ 'tags.name_required' | translate }}</mat-error>
          }
          @if (form.get('name')?.hasError('maxlength')) {
            <mat-error>{{ 'tags.name_max_length' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'tags.description' | translate }}</mat-label>
          <textarea
            matInput
            formControlName="description"
            rows="3"
            maxlength="500"
          ></textarea>
          @if (form.get('description')?.hasError('maxlength')) {
            <mat-error>{{ 'tags.description_max_length' | translate }}</mat-error>
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
export class TagDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<TagDialogComponent>);
  private readonly fb = inject(FormBuilder);
  readonly data = inject<Tag | undefined>(MAT_DIALOG_DATA);

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
