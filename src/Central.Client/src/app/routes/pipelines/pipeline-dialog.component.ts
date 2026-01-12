import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { TranslateModule } from '@ngx-translate/core';
import {
  Pipeline,
  CreatePipelineRequest,
  UpdatePipelineRequest,
  PipelineStep,
} from '../../models/pipeline.model';
import { Store } from '@ngxs/store';
import { TasksState, TasksActions } from '../../core/states/tasks.state';
import { Task } from '../../models/task.model';

@Component({
  selector: 'app-pipeline-dialog',
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
    MatIconModule,
    MatCardModule,
    DragDropModule,
    TranslateModule,
  ],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit Pipeline' : 'Create Pipeline' }}</h2>
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-dialog-content>
        <div class="form-section">
          <h3>General</h3>

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

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Trigger State (Optional)</mat-label>
            <mat-select formControlName="triggerState">
              <mat-option [value]="null">Manual (No automatic trigger)</mat-option>
              <mat-option value="Imported">On Document Imported</mat-option>
              <mat-option value="Processing">On Document Processing</mat-option>
              <mat-option value="Review">On Document Review</mat-option>
              <mat-option value="Approved">On Document Approved</mat-option>
              <mat-option value="Failed">On Document Failed</mat-option>
              <mat-option value="Processed">On Document Processed</mat-option>
            </mat-select>
            <mat-hint>Leave empty for manual execution only</mat-hint>
          </mat-form-field>
        </div>

        <div class="form-section">
          <div class="section-header">
            <h3>Pipeline Steps</h3>
            <div class="step-actions">
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
                    <span class="step-number">{{ i + 1 }}</span>
                    <mat-form-field appearance="outline" class="step-type-field">
                      <mat-label>Step Type</mat-label>
                      <mat-select formControlName="stepType" (selectionChange)="onStepTypeChange(i)">
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
                    @if (getStepType(i) === 'TaskStep') {
                      <mat-form-field appearance="outline" class="full-width">
                        <mat-label>Task</mat-label>
                        <mat-select formControlName="taskId" required>
                          @for (task of enabledTasks(); track task.id) {
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

    .form-section {
      margin-bottom: 32px;

      &:last-child {
        margin-bottom: 0;
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
        box-shadow: 0 5px 5px -3px rgba(0, 0, 0, 0.2),
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

    mat-dialog-content {
      min-width: 700px;
      max-height: 70vh;
      overflow-y: auto;
    }

    ::ng-deep .cdk-drop-list-dragging .step-card:not(.cdk-drag-placeholder) {
      transition: transform 250ms cubic-bezier(0, 0, 0.2, 1);
    }
  `],
})
export class PipelineDialogComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<PipelineDialogComponent>);
  private readonly store = inject(Store);
  readonly data = inject<Pipeline | undefined>(MAT_DIALOG_DATA);

  form!: FormGroup;
  availableTasks = this.store.selectSignal(TasksState.tasks);
  enabledTasks = computed(() => this.availableTasks().filter(t => t.enabled));

  get stepsArray(): FormArray {
    return this.form.get('steps') as FormArray;
  }

  ngOnInit() {
    this.store.dispatch(new TasksActions.Load());

    this.form = this.fb.group({
      name: [this.data?.name || '', Validators.required],
      description: [this.data?.description || ''],
      enabled: [this.data?.enabled ?? true],
      triggerState: [this.data?.triggerState || null],
      steps: this.fb.array([]),
    });

    if (this.data?.steps) {
      this.data.steps.forEach((step) => {
        this.stepsArray.push(this.createStepFormGroup(step));
      });
    }
  }

  createStepFormGroup(step?: PipelineStep): FormGroup {
    const stepType = step?.stepType || 'TaskStep';
    const group = this.fb.group({
      stepType: [stepType, Validators.required],
      order: [step?.order || this.stepsArray.length + 1],
      taskId: [step?.taskId || null],
      waitDurationSeconds: [step?.waitDurationSeconds || null],
    });

    this.updateStepValidation(group, stepType);
    return group;
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
    this.stepsArray.removeAt(index);
    this.updateStepOrders();
  }

  onStepTypeChange(index: number) {
    const stepGroup = this.stepsArray.at(index) as FormGroup;
    const stepType = stepGroup.get('stepType')?.value;

    stepGroup.patchValue({
      taskId: null,
      waitDurationSeconds: null,
    });

    this.updateStepValidation(stepGroup, stepType);
  }

  private updateStepValidation(stepGroup: FormGroup, stepType: string) {
    const taskIdControl = stepGroup.get('taskId');
    const waitControl = stepGroup.get('waitDurationSeconds');

    if (stepType === 'TaskStep') {
      taskIdControl?.setValidators(Validators.required);
      waitControl?.clearValidators();
    } else {
      taskIdControl?.clearValidators();
      waitControl?.setValidators([Validators.required, Validators.min(1)]);
    }

    taskIdControl?.updateValueAndValidity();
    waitControl?.updateValueAndValidity();
  }

  onStepDrop(event: CdkDragDrop<FormGroup[]>) {
    const stepsArray = this.stepsArray;
    const controls = stepsArray.controls;

    moveItemInArray(controls, event.previousIndex, event.currentIndex);
    stepsArray.clear();
    controls.forEach((control) => stepsArray.push(control));

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
      const formValue = this.form.value;
      const steps: PipelineStep[] = formValue.steps.map((step: any) => ({
        name: step.stepType === 'TaskStep' ? `Task Step ${step.order}` : `Wait Step ${step.order}`,
        stepType: step.stepType,
        order: step.order,
        taskId: step.stepType === 'TaskStep' ? step.taskId : null,
        waitDurationSeconds: step.stepType === 'WaitStep' ? step.waitDurationSeconds : null,
      }));

      const request: CreatePipelineRequest | UpdatePipelineRequest = {
        name: formValue.name.trim(),
        description: formValue.description?.trim() || null,
        enabled: formValue.enabled,
        triggerState: formValue.triggerState || null,
        steps,
      };

      this.dialogRef.close(request);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
