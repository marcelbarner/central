import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSliderModule } from '@angular/material/slider';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { TranslateModule } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Task, CreateTaskRequest, UpdateTaskRequest } from '../../models/task.model';
import { TaskService } from '../../services/task.service';
import { ToolsSelectorComponent } from '../../shared/components/tools-selector/tools-selector.component';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatSliderModule,
    MatCardModule,
    MatIconModule,
    MatToolbarModule,
    TranslateModule,
    ToolsSelectorComponent,
  ],
  template: `
    <mat-toolbar color="primary">
      <button mat-icon-button (click)="onCancel()">
        <mat-icon>arrow_back</mat-icon>
      </button>
      <span>{{ isEditMode ? 'Edit Task' : 'Create Task' }}</span>
    </mat-toolbar>

    <div class="container">
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
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
                <mat-option value="wait">Wait</mat-option>
                <mat-option value="openai">OpenAI</mat-option>
                <mat-option value="document_intelligence">Document Intelligence</mat-option>
              </mat-select>
              @if (form.get('taskType')?.hasError('required')) {
                <mat-error>Task type is required</mat-error>
              }
            </mat-form-field>

            @if (form.get('taskType')?.value === 'wait') {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Wait Duration (seconds)</mat-label>
                <input matInput type="number" formControlName="waitDuration" min="1" />
              </mat-form-field>
            }

            @if (form.get('taskType')?.value === 'openai') {
              <div class="configuration-section">
                <h3>Azure OpenAI Configuration</h3>
                
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Azure Endpoint</mat-label>
                  <input matInput formControlName="azureEndpoint" placeholder="https://your-resource.openai.azure.com/" />
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Azure API Key</mat-label>
                  <input matInput type="password" formControlName="azureApiKey" />
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Model/Deployment</mat-label>
                  <input matInput formControlName="azureModelOrDeployment" placeholder="gpt-4" />
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>System Prompt</mat-label>
                  <textarea matInput formControlName="systemPrompt" rows="4" placeholder="You are a helpful assistant..."></textarea>
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>User Prompt</mat-label>
                  <textarea matInput formControlName="userPrompt" rows="4" placeholder="Analyze this document..."></textarea>
                </mat-form-field>

                <div class="slider-field">
                  <span class="slider-label">Temperature: {{ form.get('temperature')?.value }}</span>
                  <mat-slider min="0" max="2" step="0.1" discrete>
                    <input matSliderThumb formControlName="temperature" />
                  </mat-slider>
                </div>

                <div class="slider-field">
                  <span class="slider-label">Max Tokens: {{ form.get('maxTokens')?.value }}</span>
                  <mat-slider min="100" max="4000" step="100" discrete>
                    <input matSliderThumb formControlName="maxTokens" />
                  </mat-slider>
                </div>

                <h3>Allowed Tools</h3>
                <app-tools-selector formControlName="allowedTools" />
              </div>
            }

            @if (form.get('taskType')?.value === 'document_intelligence') {
              <div class="configuration-section">
                <h3>Azure Document Intelligence Configuration</h3>
                
                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Azure Endpoint</mat-label>
                  <input matInput formControlName="azureEndpoint" placeholder="https://your-resource.cognitiveservices.azure.com/" />
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Azure API Key</mat-label>
                  <input matInput type="password" formControlName="azureApiKey" />
                </mat-form-field>

                <mat-form-field appearance="outline" class="full-width">
                  <mat-label>Model ID</mat-label>
                  <input matInput formControlName="azureModelOrDeployment" placeholder="prebuilt-layout" />
                </mat-form-field>
              </div>
            }

            <div class="form-actions">
              <button mat-button type="button" (click)="onCancel()">Cancel</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
                {{ isEditMode ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [
    `
      .container {
        max-width: 800px;
        margin: 24px auto;
        padding: 0 16px;
      }

      mat-card {
        margin-bottom: 24px;
      }

      .full-width {
        width: 100%;
        margin-bottom: 16px;
      }

      .configuration-section {
        margin: 24px 0;
        padding: 16px;
        background-color: #f5f5f5;
        border-radius: 4px;
      }

      .configuration-section h3 {
        margin: 0 0 16px 0;
        font-size: 16px;
        font-weight: 500;
      }

      .slider-field {
        margin-bottom: 24px;
      }

      .slider-field .slider-label {
        display: block;
        margin-bottom: 8px;
        font-size: 14px;
        color: rgba(0, 0, 0, 0.6);
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        margin-top: 24px;
      }
    `,
  ],
})
export class TaskFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);

  form!: FormGroup;
  isEditMode = false;
  taskId?: number;

  ngOnInit() {
    this.initializeForm();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.taskId = +id;
      this.loadTask(this.taskId);
    }
  }

  private initializeForm() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      taskType: ['', Validators.required],
      waitDuration: [5],
      azureEndpoint: [''],
      azureApiKey: [''],
      azureModelOrDeployment: [''],
      systemPrompt: [''],
      userPrompt: [''],
      temperature: [0.7],
      maxTokens: [1000],
      allowedTools: [[]],
    });
  }

  private loadTask(id: number) {
    this.taskService.getById(id).subscribe((task) => {
      this.form.patchValue({
        name: task.name,
        description: task.description,
        taskType: task.taskType,
        waitDuration: task.configuration?.waitDuration,
        azureEndpoint: task.configuration?.azureEndpoint,
        azureApiKey: task.configuration?.azureApiKey,
        azureModelOrDeployment: task.configuration?.azureModelOrDeployment,
        systemPrompt: task.configuration?.systemPrompt,
        userPrompt: task.configuration?.userPrompt,
        temperature: task.configuration?.temperature,
        maxTokens: task.configuration?.maxTokens,
        allowedTools: task.configuration?.allowedTools || [],
      });
    });
  }

  onTaskTypeChange() {
    const taskType = this.form.get('taskType')?.value;
    
    if (taskType === 'wait') {
      this.form.patchValue({
        azureEndpoint: '',
        azureApiKey: '',
        azureModelOrDeployment: '',
        systemPrompt: '',
        userPrompt: '',
        temperature: 0.7,
        maxTokens: 1000,
        allowedTools: [],
      });
    } else if (taskType === 'openai') {
      this.form.patchValue({
        waitDuration: 5,
      });
    } else if (taskType === 'document_intelligence') {
      this.form.patchValue({
        waitDuration: 5,
        systemPrompt: '',
        userPrompt: '',
        temperature: 0.7,
        maxTokens: 1000,
        allowedTools: [],
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const taskType = this.form.value.taskType;
      let configuration: any = {};

      if (taskType === 'wait') {
        configuration = {
          waitDuration: this.form.value.waitDuration,
        };
      } else if (taskType === 'openai') {
        configuration = {
          azureEndpoint: this.form.value.azureEndpoint,
          azureApiKey: this.form.value.azureApiKey,
          azureModelOrDeployment: this.form.value.azureModelOrDeployment,
          systemPrompt: this.form.value.systemPrompt,
          userPrompt: this.form.value.userPrompt,
          temperature: this.form.value.temperature,
          maxTokens: this.form.value.maxTokens,
          allowedTools: this.form.value.allowedTools,
        };
      } else if (taskType === 'document_intelligence') {
        configuration = {
          azureEndpoint: this.form.value.azureEndpoint,
          azureApiKey: this.form.value.azureApiKey,
          azureModelOrDeployment: this.form.value.azureModelOrDeployment,
        };
      }

      const request: CreateTaskRequest | UpdateTaskRequest = {
        name: this.form.value.name,
        description: this.form.value.description,
        taskType,
        enabled: true,
        configuration,
      };

      if (this.isEditMode && this.taskId) {
        this.taskService.update(this.taskId, request as UpdateTaskRequest).subscribe(() => {
          this.router.navigate(['/tasks']);
        });
      } else {
        this.taskService.create(request as CreateTaskRequest).subscribe(() => {
          this.router.navigate(['/tasks']);
        });
      }
    }
  }

  onCancel() {
    this.location.back();
  }
}
