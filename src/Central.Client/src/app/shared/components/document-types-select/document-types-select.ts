import { Component, EventEmitter, inject, input, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DocumentTypesState } from '@core';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-document-types-select',
  standalone: true,
  imports: [MtxSelectModule, MatFormFieldModule, FormsModule],
  template: `
    <mat-form-field appearance="outline" class="w-full">
      @if (!hideLabel()) {
        <mat-label>Select Document Types</mat-label>
      }
      <mtx-select
        [items]="documentTypes()"
        placeholder="Select Document Types"
        [multiple]="false"
        bindLabel="name"
        bindValue="id",
        [ngModel]="selected"
        (ngModelChange)="onSelectedDocumentTypesChange($event)"
      />
    </mat-form-field>
  `,
})
export class DocumentTypesSelect {
  private readonly store = inject(Store);
  protected documentTypes = this.store.selectSignal(DocumentTypesState.documentTypes);

  hideLabel = input<boolean>(false);

  @Input()
  set selectedDocumentTypes(value: number|null) {
    this.selected = value ?? undefined;
  }
  selected?: number;
  @Output()
  selectedDocumentTypesChange = new EventEmitter<number>();

  onSelectedDocumentTypesChange(selected: number) {
    this.selected = selected;
    this.selectedDocumentTypesChange.emit(this.selected ?? null);
  }
}
