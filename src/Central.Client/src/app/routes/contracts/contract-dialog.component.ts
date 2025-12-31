import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { Contract, ContractState } from '../../models/contract.model';
import { CorrespondentsState, CorrespondentsActions } from '@core';

@Component({
  selector: 'app-contract-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>
      {{ (data ? 'contracts.edit' : 'contracts.create') | translate }}
    </h2>

    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-dialog-content class="space-y-4">
        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.name' | translate }}</mat-label>
          <input matInput formControlName="name" required maxlength="200" />
          @if (form.get('name')?.hasError('required')) {
            <mat-error>{{ 'contracts.nameRequired' | translate }}</mat-error>
          }
          @if (form.get('name')?.hasError('maxlength')) {
            <mat-error>{{ 'contracts.nameMaxLength' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.description' | translate }}</mat-label>
          <textarea
            matInput
            formControlName="description"
            rows="3"
            maxlength="1000"
          ></textarea>
          @if (form.get('description')?.hasError('maxlength')) {
            <mat-error>{{ 'contracts.descriptionMaxLength' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.state' | translate }}</mat-label>
          <mat-select formControlName="state" required>
            @for (state of contractStates; track state) {
              <mat-option [value]="state">
                {{ 'contracts.states.' + state | translate }}
              </mat-option>
            }
          </mat-select>
          @if (form.get('state')?.hasError('required')) {
            <mat-error>{{ 'contracts.stateRequired' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.correspondent' | translate }}</mat-label>
          <mat-select formControlName="correspondentId">
            <mat-option [value]="null">{{ 'common.none' | translate }}</mat-option>
            @for (correspondent of correspondents(); track correspondent.id) {
              <mat-option [value]="correspondent.id">
                {{ correspondent.name }}
              </mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.customerId' | translate }}</mat-label>
          <input matInput formControlName="customerId" maxlength="100" />
          @if (form.get('customerId')?.hasError('maxlength')) {
            <mat-error>{{ 'contracts.customerIdMaxLength' | translate }}</mat-error>
          }
        </mat-form-field>

        <mat-form-field class="w-full">
          <mat-label>{{ 'contracts.contractId' | translate }}</mat-label>
          <input matInput formControlName="contractId" maxlength="100" />
          @if (form.get('contractId')?.hasError('maxlength')) {
            <mat-error>{{ 'contracts.contractIdMaxLength' | translate }}</mat-error>
          }
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end" class="space-x-2">
        <button mat-button type="button" (click)="onCancel()">
          {{ 'common.cancel' | translate }}
        </button>
        <button
          mat-raised-button
          color="primary"
          type="submit"
          [disabled]="form.invalid || form.pristine"
        >
          {{ 'common.save' | translate }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: `
    :host {
      display: block;
    }

    mat-dialog-content {
      min-width: 500px;
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .w-full {
      width: 100%;
    }
  `,
})
export class ContractDialogComponent implements OnInit {
  private readonly dialogRef = inject(MatDialogRef<ContractDialogComponent>);
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(Store);
  readonly data = inject<Contract | undefined>(MAT_DIALOG_DATA);

  form: FormGroup;
  contractStates = Object.values(ContractState);
  correspondents = this.store.selectSignal(CorrespondentsState.correspondents);

  constructor() {
    this.form = this.fb.group({
      name: [this.data?.name || '', [Validators.required, Validators.maxLength(200)]],
      description: [this.data?.description || '', [Validators.maxLength(1000)]],
      state: [this.data?.state || ContractState.Draft, [Validators.required]],
      correspondentId: [this.data?.correspondentId || null],
      customerId: [this.data?.customerId || '', [Validators.maxLength(100)]],
      contractId: [this.data?.contractId || '', [Validators.maxLength(100)]],
    });
  }

  ngOnInit() {
    this.store.dispatch(new CorrespondentsActions.Load());
  }

  onSubmit() {
    if (this.form.valid) {
      const value = this.form.value;
      this.dialogRef.close({
        name: value.name.trim(),
        description: value.description?.trim() || undefined,
        state: value.state,
        correspondentId: value.correspondentId || undefined,
        customerId: value.customerId?.trim() || undefined,
        contractId: value.contractId?.trim() || undefined,
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
