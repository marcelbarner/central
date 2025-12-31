import { Component, EventEmitter, inject, input, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CorrespondentsState, TagsState } from '@core';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-correspondents-select',
  standalone: true,
  imports: [MtxSelectModule, MatFormFieldModule, FormsModule],
  template: `
    <mat-form-field appearance="outline" class="w-full">
      @if (!hideLabel()) {
        <mat-label>Select Correspondents</mat-label>
      }
      <mtx-select
        [items]="correspondents()"
        placeholder="Select Correspondents"
        multiple="false"
        bindLabel="name"
        bindValue="id",
        [ngModel]="selected"
        (ngModelChange)="onSelectedCorrespondentsChange($event)"
      />
    </mat-form-field>
  `,
})
export class CorrespondentsSelect {
  private readonly store = inject(Store);
  protected correspondents = this.store.selectSignal(CorrespondentsState.correspondents);

  hideLabel = input<boolean>(false);

  @Input()
  set selectedCorrespondents(value: number|null) {
    this.selected = value ?? undefined;
  }
  @Output()
  selectedCorrespondentsChange = new EventEmitter<number>();

  selected?: number;

  onSelectedCorrespondentsChange(selected: number) {
    this.selected = selected;
    this.selectedCorrespondentsChange.emit(this.selected);
  }
}
