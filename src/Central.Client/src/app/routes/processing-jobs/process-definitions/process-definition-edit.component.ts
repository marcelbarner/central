import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { PageHeader } from '@shared';
import { ProcessingService } from '../services/processing.service';
import { DocumentState, StepType, ProcessingStep } from '../models/process.models';

@Component({
  selector: 'app-process-definition-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-content>
        <div class="header-actions">
          <button mat-raised-button (click)="navigateBack()">
            <mat-icon>close</mat-icon>
            {{ 'cancel' | translate }}
          </button>
          <button mat-raised-button color="primary" (click)="save()" [disabled]="form.invalid || saving()">
            <mat-icon>save</mat-icon>
            {{ saving() ? ('saving' | translate) : ('save' | translate) }}
          </button>
        </div>

        @if (loading()) {
          <div class="loading">
            {{ 'processing.loading' | translate }}
          </div>
        } @else {
          <form [formGroup]="form">
            <div class="form-container">
              <div class="form-section">
                <mat-card class="info-card">
                  <mat-card-header>
                    <mat-card-title>Basic Information</mat-card-title>
                  </mat-card-header>
                  <mat-card-content>
                    <mat-form-field appearance="outline" class="full-width">
                      <mat-label>Name</mat-label>
                      <input matInput formControlName="name" placeholder="Extract Document Metadata" required>
                      <mat-error *ngIf="form.get('name')?.hasError('required')">
                        Name is required
                      </mat-error>
                    </mat-form-field>

                    <mat-form-field appearance="outline" class="full-width">
                      <mat-label>Description</mat-label>
                      <textarea matInput formControlName="description" rows="3" placeholder="Describe what this process does..."></textarea>
                    </mat-form-field>

                    <div class="form-row">
                      <mat-form-field appearance="outline" class="half-width">
                        <mat-label>Trigger State</mat-label>
                        <mat-select formControlName="triggerState" required>
                          @for (state of documentStates; track state) {
                            <mat-option [value]="state">{{ state }}</mat-option>
                          }
                        </mat-select>
                      </mat-form-field>

                      <div class="checkbox-wrapper">
                        <mat-checkbox formControlName="enabled">
                          Enable automatic processing
                        </mat-checkbox>
                      </div>
                    </div>
                  </mat-card-content>
                </mat-card>

                <mat-card class="steps-card">
                  <mat-card-header>
                    <mat-card-title>Processing Steps</mat-card-title>
                    <button mat-raised-button color="primary" type="button" (click)="addStep()" class="add-step-btn">
                      <mat-icon>add</mat-icon>
                      Add Step
                    </button>
                  </mat-card-header>
                  <mat-card-content>
                    @if (steps.length === 0) {
                      <div class="no-steps">
                        No steps defined. Click "Add Step" to create your first processing step.
                      </div>
                    }

                    <div formArrayName="steps" class="steps-list">
                      @for (step of steps.controls; track $index; let i = $index) {
                        <mat-card class="step-card" [formGroupName]="i">
                          <mat-card-header>
                            <mat-card-title>
                              <mat-icon class="drag-handle">drag_indicator</mat-icon>
                              Step {{ i + 1 }}
                            </mat-card-title>
                            <button mat-icon-button color="warn" type="button" (click)="removeStep(i)">
                              <mat-icon>delete</mat-icon>
                            </button>
                          </mat-card-header>
                          <mat-card-content>
                            <div class="form-row">
                              <mat-form-field appearance="outline" class="half-width">
                                <mat-label>Step Name</mat-label>
                                <input matInput formControlName="name" placeholder="Extract Text" required>
                              </mat-form-field>

                              <mat-form-field appearance="outline" class="half-width">
                                <mat-label>Step Type</mat-label>
                                <mat-select formControlName="stepType" required>
                                  @for (type of stepTypes; track type) {
                                    <mat-option [value]="type">{{ type }}</mat-option>
                                  }
                                </mat-select>
                              </mat-form-field>
                            </div>

                            <div class="form-row">
                              <mat-form-field appearance="outline" class="half-width">
                                <mat-label>Azure Endpoint</mat-label>
                                <input matInput formControlName="azureEndpoint" placeholder="https://your-resource.cognitiveservices.azure.com">
                              </mat-form-field>

                              <mat-form-field appearance="outline" class="half-width">
                                <mat-label>Azure API Key</mat-label>
                                <input matInput type="password" formControlName="azureApiKey" placeholder="Enter API key">
                              </mat-form-field>
                            </div>

                            <mat-form-field appearance="outline" class="full-width">
                              <mat-label>Model/Deployment</mat-label>
                              <input matInput formControlName="azureModelOrDeployment" placeholder="gpt-4 or prebuilt-layout">
                            </mat-form-field>

                            @if (step.get('stepType')?.value === 'AzureOpenAI') {
                              <mat-form-field appearance="outline" class="full-width">
                                <mat-label>Prompt</mat-label>
                                <textarea matInput formControlName="prompt" rows="3" placeholder="You are a helpful AI assistant..."></textarea>
                              </mat-form-field>

                              <div class="tools-section">
                                <h4>Available Tools</h4>
                                <mat-checkbox formControlName="enableSetTitle">
                                  <strong>Set Document Title</strong>
                                  <span class="tool-description">Allows the AI to update the document title based on content analysis</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableSetContract">
                                  <strong>Set Contract</strong>
                                  <span class="tool-description">Allows the AI to assign a contract to the document</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableSetCorrespondent">
                                  <strong>Set Correspondent</strong>
                                  <span class="tool-description">Allows the AI to assign a correspondent to the document</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableSetDocumentType">
                                  <strong>Set Document Type</strong>
                                  <span class="tool-description">Allows the AI to classify the document by type</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableSetTags">
                                  <strong>Set Tags</strong>
                                  <span class="tool-description">Allows the AI to assign tags to the document</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableSetContent">
                                  <strong>Set Content</strong>
                                  <span class="tool-description">Allows the AI to update the document's text content</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetContent">
                                  <strong>Get Document Content</strong>
                                  <span class="tool-description">Allows the AI to access the content of the current document for analysis</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetDocument">
                                  <strong>Get Document</strong>
                                  <span class="tool-description">Allows the AI to retrieve detailed information about a specific document</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetSimilar">
                                  <strong>Get Similar Documents</strong>
                                  <span class="tool-description">Provides the AI with examples of existing document titles for consistency</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetContracts">
                                  <strong>Get Contracts</strong>
                                  <span class="tool-description">Provides the AI with a list of available contracts</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetDocumentTypes">
                                  <strong>Get Document Types</strong>
                                  <span class="tool-description">Provides the AI with a list of available document types</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetCorrespondents">
                                  <strong>Get Correspondents</strong>
                                  <span class="tool-description">Provides the AI with a list of available correspondents</span>
                                </mat-checkbox>
                                <mat-checkbox formControlName="enableGetTags">
                                  <strong>Get Tags</strong>
                                  <span class="tool-description">Provides the AI with a list of available tags</span>
                                </mat-checkbox>
                              </div>
                            }

                            <mat-form-field appearance="outline" class="full-width">
                              <mat-label>Additional Configuration (JSON)</mat-label>
                              <textarea matInput formControlName="additionalConfig" rows="2" placeholder='{"key": "value"}'></textarea>
                            </mat-form-field>
                          </mat-card-content>
                        </mat-card>
                      }
                    </div>
                  </mat-card-content>
                </mat-card>
              </div>

              <div class="help-section">
                <mat-card class="help-card">
                  <mat-card-header>
                    <mat-card-title>Help & Tips</mat-card-title>
                  </mat-card-header>
                  <mat-card-content>
                    <div class="help-item">
                      <h4>Trigger State</h4>
                      <p>
                        Select the document state that will automatically trigger this process.
                        Common choice: <strong>Imported</strong>
                      </p>
                    </div>

                    <div class="help-item">
                      <h4>Step Types</h4>
                      <p><strong>Azure OpenAI:</strong> Use GPT models for text analysis, summarization, or extraction</p>
                      <p><strong>Azure Document Intelligence:</strong> Extract text, tables, and structure from documents</p>
                    </div>

                    <div class="help-item">
                      <h4>Step Order</h4>
                      <p>
                        Steps execute sequentially from top to bottom. Drag to reorder.
                      </p>
                    </div>
                  </mat-card-content>
                </mat-card>
              </div>
            </div>
          </form>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .header-actions {
      display: flex;
      justify-content: flex-end;
      gap: 12px;
      margin-bottom: 20px;
    }

    .loading {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .form-container {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 20px;
      align-items: start;
    }

    .form-section {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .info-card mat-card-header {
      margin-bottom: 16px;
    }

    .steps-card mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;

      .add-step-btn {
        margin-left: auto;
      }
    }

    .full-width {
      width: 100%;
    }

    .half-width {
      flex: 1;
      min-width: 0;
    }

    .form-row {
      display: flex;
      gap: 16px;
      align-items: flex-start;
    }

    .checkbox-wrapper {
      display: flex;
      align-items: center;
      padding-top: 8px;
    }

    .no-steps {
      text-align: center;
      padding: 40px;
      color: #666;
    }

    .steps-list {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .step-card {
      mat-card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;

        mat-card-title {
          display: flex;
          align-items: center;
          gap: 8px;
          font-size: 16px;
          font-weight: 500;
        }

        .drag-handle {
          cursor: move;
          color: #999;
        }
      }
    }

    .help-section {
      position: sticky;
      top: 20px;
    }

    .help-card {
      mat-card-header {
        margin-bottom: 16px;
      }
    }

    .help-item {
      margin-bottom: 20px;

      &:last-child {
        margin-bottom: 0;
      }

      h4 {
        color: #1976d2;
        font-size: 14px;
        font-weight: 500;
        margin: 0 0 8px 0;
      }

      p {
        font-size: 13px;
        color: #666;
        margin: 4px 0;
        line-height: 1.5;
      }
    }

    .tools-section {
      margin: 16px 0;
      padding: 16px;
      background: #f5f5f5;
      border-radius: 4px;

      h4 {
        margin: 0 0 12px 0;
        font-size: 14px;
        font-weight: 500;
        color: #333;
      }

      mat-checkbox {
        display: block;
        margin-bottom: 12px;

        &:last-child {
          margin-bottom: 0;
        }

        .tool-description {
          display: block;
          font-size: 12px;
          color: #666;
          margin-left: 28px;
          margin-top: 4px;
        }
      }
    }
  `]
})
export class ProcessDefinitionEditComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly processingService = inject(ProcessingService);

  form!: FormGroup;
  loading = signal(false);
  saving = signal(false);
  isEditMode = signal(false);
  processId = signal<number | null>(null);

  documentStates = Object.values(DocumentState);
  stepTypes = Object.values(StepType);

  ngOnInit(): void {
    this.initializeForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.processId.set(+id);
      this.loadProcess(+id);
    }
  }

  navigateBack(): void {
    this.router.navigate(['/processing-jobs/definitions']);
  }

  initializeForm(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      enabled: [true],
      triggerState: [DocumentState.Imported, Validators.required],
      steps: this.fb.array([])
    });
  }

  get steps(): FormArray {
    return this.form.get('steps') as FormArray;
  }

  loadProcess(id: number): void {
    this.loading.set(true);

    this.processingService.getProcessDefinition(id).subscribe({
      next: (process) => {
        this.form.patchValue({
          name: process.name,
          description: process.description,
          enabled: process.enabled,
          triggerState: process.triggerState
        });

        // Clear existing steps and add loaded ones
        this.steps.clear();
        process.steps.forEach(step => {
          this.steps.push(this.createStepFormGroup(step));
        });

        this.loading.set(false);
      },
      error: (err: unknown) => {
        console.error('Error loading process:', err);
        this.loading.set(false);
        this.router.navigate(['/processing-jobs/definitions']);
      }
    });
  }

  createStepFormGroup(step?: ProcessingStep): FormGroup {
    // Parse configuration to extract enabled tools
    let enableSetTitle = false;
    let enableSetContract = false;
    let enableSetCorrespondent = false;
    let enableSetDocumentType = false;
    let enableSetTags = false;
    let enableSetContent = false;
    let enableGetContent = false;
    let enableGetDocument = false;
    let enableGetSimilar = false;
    let enableGetContracts = false;
    let enableGetDocumentTypes = false;
    let enableGetCorrespondents = false;
    let enableGetTags = false;

    if (step?.configuration) {
      try {
        const tools = JSON.parse(step.configuration) as string[];
        enableSetTitle = tools.includes('SetDocumentTitle');
        enableSetContract = tools.includes('SetContract');
        enableSetCorrespondent = tools.includes('SetCorrespondent');
        enableSetDocumentType = tools.includes('SetDocumentType');
        enableSetTags = tools.includes('SetTags');
        enableSetContent = tools.includes('SetContent');
        enableGetContent = tools.includes('GetDocumentContent');
        enableGetDocument = tools.includes('GetDocument');
        enableGetSimilar = tools.includes('GetSimilarDocuments');
        enableGetContracts = tools.includes('GetContracts');
        enableGetDocumentTypes = tools.includes('GetDocumentTypes');
        enableGetCorrespondents = tools.includes('GetCorrespondents');
        enableGetTags = tools.includes('GetTags');
      } catch {
        // Invalid JSON, use defaults
      }
    }

    return this.fb.group({
      id: [step?.id || 0],
      name: [step?.name || '', Validators.required],
      description: [step?.description || ''],
      stepType: [step?.stepType || StepType.AzureDocumentIntelligence, Validators.required],
      order: [step?.order || this.steps.length],
      azureEndpoint: [step?.azureEndpoint || ''],
      azureApiKey: [step?.azureApiKey || ''],
      azureModelOrDeployment: [step?.azureModelOrDeployment || ''],
      prompt: [step?.prompt || ''],
      additionalConfig: [step?.configuration || ''],
      enableSetTitle: [enableSetTitle],
      enableSetContract: [enableSetContract],
      enableSetCorrespondent: [enableSetCorrespondent],
      enableSetDocumentType: [enableSetDocumentType],
      enableSetTags: [enableSetTags],
      enableSetContent: [enableSetContent],
      enableGetContent: [enableGetContent],
      enableGetDocument: [enableGetDocument],
      enableGetSimilar: [enableGetSimilar],
      enableGetContracts: [enableGetContracts],
      enableGetDocumentTypes: [enableGetDocumentTypes],
      enableGetCorrespondents: [enableGetCorrespondents],
      enableGetTags: [enableGetTags]
    });
  }

  addStep(): void {
    this.steps.push(this.createStepFormGroup());
  }

  removeStep(index: number): void {
    this.steps.removeAt(index);
    this.updateStepOrders();
  }

  onStepReordered(event: any): void {
    const step = this.steps.at(event.previousIndex);
    this.steps.removeAt(event.previousIndex);
    this.steps.insert(event.currentIndex, step);
    this.updateStepOrders();
  }

  updateStepOrders(): void {
    this.steps.controls.forEach((control, index) => {
      control.get('order')?.setValue(index);
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const formValue = this.form.value;

    const request = {
      name: formValue.name!,
      description: formValue.description!,
      enabled: formValue.enabled!,
      triggerState: formValue.triggerState!,
      steps: formValue.steps!.map((step: any) => {
        // Build tool configuration array from checkboxes
        const tools: string[] = [];
        if (step.stepType === 'AzureOpenAI') {
          if (step.enableSetTitle) {
            tools.push('SetDocumentTitle');
          }
          if (step.enableSetContract) {
            tools.push('SetContract');
          }
          if (step.enableSetCorrespondent) {
            tools.push('SetCorrespondent');
          }
          if (step.enableSetDocumentType) {
            tools.push('SetDocumentType');
          }
          if (step.enableSetTags) {
            tools.push('SetTags');
          }
          if (step.enableSetContent) {
            tools.push('SetContent');
          }
          if (step.enableGetContent) {
            tools.push('GetDocumentContent');
          }
          if (step.enableGetDocument) {
            tools.push('GetDocument');
          }
          if (step.enableGetSimilar) {
            tools.push('GetSimilarDocuments');
          }
          if (step.enableGetContracts) {
            tools.push('GetContracts');
          }
          if (step.enableGetDocumentTypes) {
            tools.push('GetDocumentTypes');
          }
          if (step.enableGetCorrespondents) {
            tools.push('GetCorrespondents');
          }
          if (step.enableGetTags) {
            tools.push('GetTags');
          }
        }

        return {
          id: step.id || 0,
          name: step.name,
          description: step.description || null,
          stepType: step.stepType,
          order: step.order,
          azureEndpoint: step.azureEndpoint || null,
          azureApiKey: step.azureApiKey || null,
          azureModelOrDeployment: step.azureModelOrDeployment || null,
          prompt: step.prompt || null,
          configuration: tools.length > 0 ? JSON.stringify(tools) : (step.additionalConfig || null)
        };
      })
    };

    this.saving.set(true);

    if (this.isEditMode()) {
      this.processingService.updateProcessDefinition(this.processId()!, request).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/processing-jobs/definitions']);
        },
        error: (err: unknown) => {
          this.saving.set(false);
          console.error('Error saving process:', err);
        }
      });
    } else {
      this.processingService.createProcessDefinition(request).subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/processing-jobs/definitions']);
        },
        error: (err: unknown) => {
          this.saving.set(false);
          console.error('Error saving process:', err);
        }
      });
    }
  }
}
