import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { TranslateModule } from '@ngx-translate/core';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.model';
import { ToolsSelectorComponent } from '../../shared/components/tools-selector/tools-selector.component';

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatSliderModule,
    TranslateModule,
    ToolsSelectorComponent,
  ],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit Task' : 'Create Task' }}</h2>
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Name</mat-label>
          <input matInput formControlName="name" required />
          @if (form.get('name')?.hasError('required')) {
            <mat-error>Name is required</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Description</mat-label>
          <textarea matInput formControlName="description" rows="3"></textarea>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Task Type</mat-label>
          <mat-select formControlName="taskType" required (selectionChange)="onTaskTypeChange()">
            <mat-option value="AzureOpenAI">Azure OpenAI</mat-option>
            <mat-option value="AzureDocumentIntelligence">Azure Document Intelligence</mat-option>
          </mat-select>
        </mat-form-field>

        <div class="checkbox-field">
          <mat-checkbox formControlName="enabled">Enabled</mat-checkbox>
        </div>

        <h3>Configuration</h3>
        <div formGroupName="configuration">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Azure Endpoint</mat-label>
            <input matInput formControlName="azureEndpoint" required />
            @if (configForm.get('azureEndpoint')?.hasError('required')) {
              <mat-error>Endpoint is required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>API Key</mat-label>
            <input matInput type="password" formControlName="azureApiKey" required />
            @if (configForm.get('azureApiKey')?.hasError('required')) {
              <mat-error>API Key is required</mat-error>
            }
          </mat-form-field>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>{{ isAzureOpenAI ? 'Model/Deployment' : 'Model ID' }}</mat-label>
            <input matInput formControlName="azureModelOrDeployment" required />
            @if (configForm.get('azureModelOrDeployment')?.hasError('required')) {
              <mat-error>{{ isAzureOpenAI ? 'Model/Deployment' : 'Model ID' }} is required</mat-error>
            }
          </mat-form-field>

          @if (isAzureOpenAI) {
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Prompt</mat-label>
              <textarea matInput formControlName="prompt" rows="4" required></textarea>
              @if (configForm.get('prompt')?.hasError('required')) {
                <mat-error>Prompt is required for Azure OpenAI</mat-error>
              }
            </mat-form-field>

            <app-tools-selector formControlName="allowedTools" />

            <div class="slider-field">
              <mat-label>Temperature: {{ configForm.get('temperature')?.value }}</mat-label>
              <mat-slider [min]="0" [max]="2" [step]="0.1" [discrete]="true">
                <input matSliderThumb formControlName="temperature" />
              </mat-slider>
            </div>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Max Tokens</mat-label>
              <input matInput type="number" formControlName="maxTokens" />
            </mat-form-field>
          }
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="!form.valid">
          {{ data ? 'Update' : 'Create' }}
        </button>
      </mat-dialog-actions>
    </form>
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .checkbox-field {
      margin-bottom: 24px;
    }

    .slider-field {
      margin-bottom: 24px;

      mat-label {
        display: block;
        margin-bottom: 8px;
        font-size: 14px;
        font-weight: 500;
      }
    }

    h3 {
      margin: 24px 0 16px 0;
      font-size: 16px;
      font-weight: 500;
    }

    mat-dialog-content {
      min-width: 500px;
      max-height: 70vh;
      overflow-y: auto;
    }
  `],
})
export class TaskDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<TaskDialogComponent>);
  readonly data = inject<Task | undefined>(MAT_DIALOG_DATA);

  form!: FormGroup;
  isAzureOpenAI = true;

  get configForm() {
    return this.form.get('configuration') as FormGroup;
  }

  ngOnInit() {
    const taskType = this.data?.taskType || 'AzureOpenAI';
    this.isAzureOpenAI = taskType === 'AzureOpenAI';

    this.form = this.fb.group({
      name: [this.data?.name || '', Validators.required],
      description: [this.data?.description || ''],
      taskType: [taskType, Validators.required],
      enabled: [this.data?.enabled ?? true],
      configuration: this.fb.group({
        azureEndpoint: [this.data?.configuration.azureEndpoint || '', Validators.required],
        azureApiKey: [this.data?.configuration.azureApiKey || '', Validators.required],
        azureModelOrDeployment: [
          this.data?.configuration.azureModelOrDeployment || '',
          Validators.required,
        ],
        prompt: [this.data?.configuration.prompt || ''],
        allowedTools: [this.data?.configuration.allowedTools || []],
        temperature: [this.data?.configuration.temperature ?? 0.7],
        maxTokens: [this.data?.configuration.maxTokens],
      }),
    });

    this.updateConfigValidation();
  }

  onTaskTypeChange() {
    const taskType = this.form.get('taskType')?.value;
    this.isAzureOpenAI = taskType === 'AzureOpenAI';
    this.updateConfigValidation();
  }

  private updateConfigValidation() {
    const promptControl = this.configForm.get('prompt');

    if (this.isAzureOpenAI) {
      promptControl?.setValidators(Validators.required);
    } else {
      promptControl?.clearValidators();
    }

    promptControl?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.form.valid) {
      const formValue = this.form.value;
      const request: CreateTaskRequest | UpdateTaskRequest = {
        name: formValue.name.trim(),
        description: formValue.description?.trim() || null,
        taskType: formValue.taskType,
        enabled: formValue.enabled,
        configuration: {
          azureEndpoint: formValue.configuration.azureEndpoint.trim(),
          azureApiKey: formValue.configuration.azureApiKey.trim(),
          azureModelOrDeployment: formValue.configuration.azureModelOrDeployment.trim(),
          prompt: formValue.configuration.prompt?.trim() || null,
          allowedTools: formValue.configuration.allowedTools || [],
          temperature: formValue.configuration.temperature,
          maxTokens: formValue.configuration.maxTokens,
        },
      };

      this.dialogRef.close(request);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
