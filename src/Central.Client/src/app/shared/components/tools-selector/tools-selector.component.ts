import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { TranslateModule } from '@ngx-translate/core';

export interface OpenAITool {
  value: string;
  label: string;
  description: string;
}

@Component({
  selector: 'app-tools-selector',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MtxSelectModule,
    MatChipsModule,
    MatIconModule,
    TranslateModule,
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ToolsSelectorComponent),
      multi: true,
    },
  ],
  template: `
    <mat-form-field appearance="outline" class="full-width">
      <mat-label>{{ label }}</mat-label>
      <mtx-select
        [items]="availableTools"
        [ngModel]="value"
        (ngModelChange)="onSelectionChange($event)"
        [multiple]="true"
        [disabled]="disabled"
        bindLabel="label"
        bindValue="value"
        placeholder="Select tools..."
      />
      <mat-hint>{{ hint }}</mat-hint>
    </mat-form-field>

    @if (value && value.length > 0) {
      <div class="selected-tools">
        <mat-chip-set>
          @for (toolValue of value; track toolValue) {
            <mat-chip [removable]="!disabled" (removed)="removeTool(toolValue)">
              {{ getToolLabel(toolValue) }}
              @if (!disabled) {
                <button matChipRemove>
                  <mat-icon>cancel</mat-icon>
                </button>
              }
            </mat-chip>
          }
        </mat-chip-set>
      </div>
    }
  `,
  styles: [`
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .selected-tools {
      margin-bottom: 16px;
    }

    mat-chip {
      margin: 4px;
    }
  `],
})
export class ToolsSelectorComponent implements ControlValueAccessor {
  @Input() label = 'Allowed Tools';
  @Input() hint = 'Select which tools the AI can use';

  availableTools: OpenAITool[] = [
    {
      value: 'SetTitle',
      label: 'Set Document Title',
      description: 'Allows the AI to update the document title based on content analysis',
    },
    {
      value: 'SetDate',
      label: 'Set Document Date',
      description: 'Allows the AI to set the document date based on content analysis',
    },
    {
      value: 'SetContract',
      label: 'Set Contract',
      description: 'Allows the AI to assign a contract to the document',
    },
    {
      value: 'SetCorrespondent',
      label: 'Set Correspondent',
      description: 'Allows the AI to assign a correspondent to the document',
    },
    {
      value: 'SetDocumentType',
      label: 'Set Document Type',
      description: 'Allows the AI to classify the document by type',
    },
    {
      value: 'SetTags',
      label: 'Set Tags',
      description: 'Allows the AI to assign tags to the document',
    },
    {
      value: 'SetContent',
      label: 'Set Content',
      description: 'Allows the AI to update the document\'s text content',
    },
    {
      value: 'GetContent',
      label: 'Get Document Content',
      description: 'Allows the AI to access the content of the current document for analysis',
    },
    {
      value: 'GetDocument',
      label: 'Get Document',
      description: 'Allows the AI to retrieve detailed information about a specific document',
    },
    {
      value: 'GetSimilar',
      label: 'Get Similar Documents',
      description: 'Provides the AI with examples of existing document titles for consistency',
    },
    {
      value: 'GetContracts',
      label: 'Get Contracts',
      description: 'Provides the AI with a list of available contracts',
    },
    {
      value: 'GetDocumentTypes',
      label: 'Get Document Types',
      description: 'Provides the AI with a list of available document types',
    },
    {
      value: 'GetCorrespondents',
      label: 'Get Correspondents',
      description: 'Provides the AI with a list of available correspondents',
    },
    {
      value: 'GetTags',
      label: 'Get Tags',
      description: 'Provides the AI with a list of available tags',
    },
    {
      value: 'CreateContract',
      label: 'Create Contract',
      description: 'Allows the AI to create new contracts',
    },
    {
      value: 'CreateCorrespondent',
      label: 'Create Correspondent',
      description: 'Allows the AI to create new correspondents',
    },
    {
      value: 'CreateDocumentType',
      label: 'Create Document Type',
      description: 'Allows the AI to create new document types',
    },
    {
      value: 'CreateTag',
      label: 'Create Tag',
      description: 'Allows the AI to create new tags',
    },
  ];

  value: string[] = [];
  disabled = false;

  private onChange: (value: string[]) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string[] | null): void {
    this.value = value || [];
  }

  registerOnChange(fn: (value: string[]) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSelectionChange(selectedValues: string[]): void {
    this.value = selectedValues;
    this.onChange(this.value);
    this.onTouched();
  }

  removeTool(toolValue: string): void {
    this.value = this.value.filter(v => v !== toolValue);
    this.onChange(this.value);
    this.onTouched();
  }

  getToolLabel(value: string): string {
    return this.availableTools.find(t => t.value === value)?.label || value;
  }
}
