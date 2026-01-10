import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { TranslateModule } from '@ngx-translate/core';
import { PageHeader, Breadcrumb } from '@shared';
import {
  Pipeline,
  CreatePipelineRequest,
  UpdatePipelineRequest,
  PipelineStep,
} from '../../models/pipeline.model';
import { PipelineService } from '../../services/pipeline.service';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-pipeline-edit',
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
    DragDropModule,
    TranslateModule,
    PageHeader,
    Breadcrumb,
  ],
  template: `
    <page-header />
    <breadcrumb />

    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ isEditMode() ? 'Edit Pipeline' : 'Create Pipeline' }}</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        @if (loading()) {
          <div class="loading">Loading...</div>
        } @else {
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-section">
              <h3>General Settings</h3>

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

              <div class="checkbox-field">
                <mat-checkbox formControlName="enabled">Enabled</mat-checkbox>
              </div>
            </div>

            <div class="form-section">
              <div class="section-header">
                <h3>Pipeline Steps</h3>
                <div class="step-actions">
                  <button
                    mat-raised-button
                    color="accent"
                    type="button"
                    (click)="addTriggerStep()"
                    [disabled]="stepsArray.length > 0 && getStepType(0) === 'TriggerStep'"
                  >
                    <mat-icon>play_arrow</mat-icon>
                    Add Trigger
                  </button>
                  <button
                    mat-raised-button
                    color="primary"
                    type="button"
                    (click)="addTaskStep()"
                    [disabled]="availableTasks().length === 0"
                  >
                    <mat-icon>psychology</mat-icon>
                    Add Task Step
                  </button>
                  <button mat-raised-button type="button" (click)="addWaitStep()">
                    <mat-icon>schedule</mat-icon>
                    Add Wait Step
                  </button>
                </div>
              </div>

              @if (availableTasks().length === 0) {
                <div class="warning-message">
                  <mat-icon>warning</mat-icon>
                  <span>No tasks available. Create tasks first before adding task steps.</span>
                </div>
              }

              <div
                cdkDropList
                (cdkDropListDropped)="onStepDrop($event)"
                formArrayName="steps"
                class="steps-container"
              >
                @if (stepsArray.length === 0) {
                  <div class="empty-steps">
                    <mat-icon>info</mat-icon>
                    <p>No steps added yet. Add task or wait steps to build your pipeline.</p>
                  </div>
                }

                @for (stepGroup of stepsArray.controls; track $index; let i = $index) {
                  <mat-card cdkDrag class="step-card">
                    <div class="drag-handle" cdkDragHandle>
                      <mat-icon>drag_indicator</mat-icon>
                    </div>

                    <div class="step-content" [formGroupName]="i">
                      <div class="step-header">
                        <span class="step-number" [class.trigger]="getStepType(i) === 'TriggerStep'">
                          {{ getStepType(i) === 'TriggerStep' ? '▶' : i + 1 }}
                        </span>
                        <mat-form-field appearance="outline" class="step-type-field">
                          <mat-label>Step Type</mat-label>
                          <mat-select
                            formControlName="stepType"
                            (selectionChange)="onStepTypeChange(i)"
                            [disabled]="i === 0 && getStepType(0) === 'TriggerStep'"
                          >
                            <mat-option value="TriggerStep" [disabled]="i !== 0">Trigger Step</mat-option>
                            <mat-option value="TaskStep">Task Step</mat-option>
                            <mat-option value="WaitStep">Wait Step</mat-option>
                          </mat-select>
                        </mat-form-field>
                        <button
                          mat-icon-button
                          color="warn"
                          type="button"
                          (click)="removeStep(i)"
                          matTooltip="Remove step"
                        >
                          <mat-icon>delete</mat-icon>
                        </button>
                      </div>

                      <div class="step-config">
                        @if (getStepType(i) === 'TriggerStep') {
                          <mat-form-field appearance="outline" class="full-width">
                            <mat-label>Trigger Event</mat-label>
                            <mat-select formControlName="triggerState" required>
                              <mat-option value="Consumed">On Document Consumed</mat-option>
                              <mat-option value="Downloaded">On Document Downloaded</mat-option>
                              <mat-option value="Processed">On Document Processed</mat-option>
                            </mat-select>
                            @if (stepGroup.get('triggerState')?.hasError('required')) {
                              <mat-error>Trigger event is required</mat-error>
                            }
                            <mat-hint>Pipeline will start automatically when this event occurs</mat-hint>
                          </mat-form-field>
                        } @else if (getStepType(i) === 'TaskStep') {
                          <mat-form-field appearance="outline" class="full-width">
                            <mat-label>Task</mat-label>
                            <mat-select formControlName="taskId" required>
                              @for (task of availableTasks(); track task.id) {
                                <mat-option [value]="task.id">
                                  {{ task.name }}
                                  <span class="task-type">({{ task.taskType }})</span>
                                </mat-option>
                              }
                            </mat-select>
                            @if (stepGroup.get('taskId')?.hasError('required')) {
                              <mat-error>Task is required for Task Step</mat-error>
                            }
                          </mat-form-field>
                        } @else {
                          <mat-form-field appearance="outline" class="full-width">
                            <mat-label>Wait Duration (seconds)</mat-label>
                            <input
                              matInput
                              type="number"
                              formControlName="waitDurationSeconds"
                              min="1"
                              required
                            />
                            @if (stepGroup.get('waitDurationSeconds')?.hasError('required')) {
                              <mat-error>Wait duration is required for Wait Step</mat-error>
                            }
                            @if (stepGroup.get('waitDurationSeconds')?.hasError('min')) {
                              <mat-error>Wait duration must be at least 1 second</mat-error>
                            }
                          </mat-form-field>
                        }
                      </div>
                    </div>
                  </mat-card>
                }
              </div>
            </div>

            <div class="form-actions">
              <button mat-button type="button" (click)="onCancel()">Cancel</button>
              <button
                mat-raised-button
                color="primary"
                type="submit"
                [disabled]="!form.valid || saving()"
              >
                {{ saving() ? 'Saving...' : isEditMode() ? 'Update' : 'Create' }}
              </button>
            </div>
          </form>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [
    `
      .full-width {
        width: 100%;
        margin-bottom: 16px;
      }

      .checkbox-field {
        margin-bottom: 24px;
      }

      .form-section {
        margin-bottom: 32px;

        &:last-of-type {
          margin-bottom: 24px;
        }

        h3 {
          margin: 0 0 16px 0;
          font-size: 16px;
          font-weight: 500;
        }
      }

      .section-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;

        h3 {
          margin: 0;
        }
      }

      .step-actions {
        display: flex;
        gap: 8px;
      }

      .warning-message {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 12px;
        background-color: #fff3cd;
        border: 1px solid #ffc107;
        border-radius: 4px;
        margin-bottom: 16px;
        color: #856404;

        mat-icon {
          font-size: 20px;
          width: 20px;
          height: 20px;
        }
      }

      .steps-container {
        display: flex;
        flex-direction: column;
        gap: 12px;
        min-height: 100px;
      }

      .empty-steps {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 32px;
        text-align: center;
        border: 2px dashed #ccc;
        border-radius: 4px;
        color: #999;

        mat-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
          margin-bottom: 8px;
        }
      }

      .step-card {
        display: flex;
        gap: 12px;
        cursor: move;

        &.cdk-drag-preview {
          opacity: 0.8;
          box-shadow:
            0 5px 5px -3px rgba(0, 0, 0, 0.2),
            0 8px 10px 1px rgba(0, 0, 0, 0.14),
            0 3px 14px 2px rgba(0, 0, 0, 0.12);
        }

        &.cdk-drag-animating {
          transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
        }
      }

      .drag-handle {
        display: flex;
        align-items: center;
        color: #999;
        cursor: move;

        mat-icon {
          font-size: 24px;
          width: 24px;
          height: 24px;
        }
      }

      .step-content {
        flex: 1;
      }

      .step-header {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 12px;
      }

      .step-number {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 32px;
        height: 32px;
        border-radius: 50%;
        background-color: #2196f3;
        color: white;
        font-weight: 500;

        &.trigger {
          background-color: #ff9800;
        }
      }

      .step-type-field {
        flex: 1;
        margin-bottom: 0;
      }

      .step-config {
        padding-left: 44px;
      }

      .task-type {
        font-size: 0.875rem;
        color: rgba(0, 0, 0, 0.6);
        margin-left: 8px;
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        padding-top: 16px;
        border-top: 1px solid #e0e0e0;
      }

      .loading {
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 40px;
      }

      ::ng-deep .cdk-drop-list-dragging .step-card:not(.cdk-drag-placeholder) {
        transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
      }
    `,
  ],
})
export class PipelineEditComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly pipelineService = inject(PipelineService);
  private readonly taskService = inject(TaskService);

  form!: FormGroup;
  availableTasks = signal<Task[]>([]);
  loading = signal(true);
  saving = signal(false);
  isEditMode = signal(false);
  pipelineId?: number;

  get stepsArray(): FormArray {
    return this.form.get('steps') as FormArray;
  }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      enabled: [true],
      steps: this.fb.array([]),
    });

    this.loadTasks();

    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.pipelineId = parseInt(id, 10);
      this.isEditMode.set(true);
      this.loadPipeline(this.pipelineId);
    } else {
      this.loading.set(false);
    }
  }

  loadTasks() {
    this.taskService.getAll().subscribe({
      next: tasks => {
        this.availableTasks.set(tasks.filter(t => t.enabled));
      },
      error: error => {
        console.error('Error loading tasks:', error);
      },
    });
  }

  loadPipeline(id: number) {
    this.pipelineService.getById(id).subscribe({
      next: pipeline => {
        this.form.patchValue({
          name: pipeline.name,
          description: pipeline.description,
          enabled: pipeline.enabled,
        });

        // Add trigger as first step if present
        if (pipeline.triggerState) {
          this.stepsArray.push(this.createStepFormGroup({
            stepType: 'TriggerStep',
            order: 1,
            taskId: null,
            waitDurationSeconds: null,
            triggerState: pipeline.triggerState,
          } as any));
        }

        pipeline.steps.forEach(step => {
          this.stepsArray.push(this.createStepFormGroup(step));
        });

        this.loading.set(false);
      },
      error: error => {
        console.error('Error loading pipeline:', error);
        this.loading.set(false);
      },
    });
  }

  createStepFormGroup(step?: PipelineStep & { triggerState?: string }): FormGroup {
    const stepType = step?.stepType || 'TaskStep';
    const group = this.fb.group({
      stepType: [stepType, Validators.required],
      order: [step?.order || this.stepsArray.length + 1],
      taskId: [step?.taskId || null],
      waitDurationSeconds: [step?.waitDurationSeconds || null],
      triggerState: [(step as any)?.triggerState || null],
    });

    this.updateStepValidation(group, stepType);
    return group;
  }

  addTriggerStep() {
    // Trigger can only be the first step
    if (this.stepsArray.length === 0) {
      const step = this.createStepFormGroup({ stepType: 'TriggerStep', triggerState: 'Consumed' } as any);
      this.stepsArray.insert(0, step);
      this.updateStepOrders();
    }
  }

  addTaskStep() {
    this.stepsArray.push(this.createStepFormGroup());
    this.updateStepOrders();
  }

  addWaitStep() {
    const step = this.createStepFormGroup({ stepType: 'WaitStep' } as PipelineStep);
    this.stepsArray.push(step);
    this.updateStepOrders();
  }

  removeStep(index: number) {
    const stepType = this.getStepType(index);
    // Allow removing trigger step
    this.stepsArray.removeAt(index);
    this.updateStepOrders();
  }

  onStepTypeChange(index: number) {
    const stepGroup = this.stepsArray.at(index) as FormGroup;
    const stepType = stepGroup.get('stepType')?.value;

    // If changing to trigger and not first step, prevent it
    if (stepType === 'TriggerStep' && index !== 0) {
      stepGroup.patchValue({ stepType: 'TaskStep' });
      return;
    }

    stepGroup.patchValue({
      taskId: null,
      waitDurationSeconds: null,
      triggerState: stepType === 'TriggerStep' ? 'Consumed' : null,
    });

    this.updateStepValidation(stepGroup, stepType);
  }

  private updateStepValidation(stepGroup: FormGroup, stepType: string) {
    const taskIdControl = stepGroup.get('taskId');
    const waitControl = stepGroup.get('waitDurationSeconds');
    const triggerControl = stepGroup.get('triggerState');

    if (stepType === 'TriggerStep') {
      taskIdControl?.clearValidators();
      waitControl?.clearValidators();
      triggerControl?.setValidators(Validators.required);
    } else if (stepType === 'TaskStep') {
      taskIdControl?.setValidators(Validators.required);
      waitControl?.clearValidators();
      triggerControl?.clearValidators();
    } else {
      taskIdControl?.clearValidators();
      waitControl?.setValidators([Validators.required, Validators.min(1)]);
      triggerControl?.clearValidators();
    }

    taskIdControl?.updateValueAndValidity();
    waitControl?.updateValueAndValidity();
    triggerControl?.updateValueAndValidity();
  }

  onStepDrop(event: CdkDragDrop<FormGroup[]>) {
    const stepsArray = this.stepsArray;
    const controls = stepsArray.controls;

    // Check if moving or moved trigger step
    const movedStepType = controls[event.previousIndex].get('stepType')?.value;
    const targetStepType = event.currentIndex === 0 ? null : controls[0]?.get('stepType')?.value;

    // Prevent moving trigger away from first position or moving anything to first if trigger is there
    if (movedStepType === 'TriggerStep' && event.currentIndex !== 0) {
      return;
    }
    if (targetStepType === 'TriggerStep' && event.currentIndex === 0) {
      return;
    }

    moveItemInArray(controls, event.previousIndex, event.currentIndex);
    stepsArray.clear();
    controls.forEach(control => stepsArray.push(control));

    this.updateStepOrders();
  }

  private updateStepOrders() {
    this.stepsArray.controls.forEach((control, index) => {
      control.patchValue({ order: index + 1 }, { emitEvent: false });
    });
  }

  getStepType(index: number): string {
    return this.stepsArray.at(index).get('stepType')?.value;
  }

  onSubmit() {
    if (this.form.valid) {
      this.saving.set(true);
      const formValue = this.form.value;
      
      // Extract trigger from first step if present
      let triggerState: string | null = null;
      const steps: PipelineStep[] = [];
      
      formValue.steps.forEach((step: any, index: number) => {
        if (step.stepType === 'TriggerStep' && index === 0) {
          triggerState = step.triggerState;
        } else if (step.stepType !== 'TriggerStep') {
          steps.push({
            stepType: step.stepType,
            order: step.order - (triggerState ? 1 : 0),
            taskId: step.stepType === 'TaskStep' ? step.taskId : null,
            waitDurationSeconds: step.stepType === 'WaitStep' ? step.waitDurationSeconds : null,
          } as PipelineStep);
        }
      });

      const request: CreatePipelineRequest | UpdatePipelineRequest = {
        name: formValue.name.trim(),
        description: formValue.description?.trim() || null,
        enabled: formValue.enabled,
        triggerState: (triggerState as any) || undefined,
        steps: steps,
      };

      const operation = this.isEditMode()
        ? this.pipelineService.update(this.pipelineId!, request)
        : this.pipelineService.create(request);

      operation.subscribe({
        next: () => {
          this.saving.set(false);
          this.router.navigate(['/pipelines']);
        },
        error: error => {
          console.error('Error saving pipeline:', error);
          this.saving.set(false);
        },
      });
    }
  }

  onCancel() {
    this.router.navigate(['/pipelines']);
  }
}
