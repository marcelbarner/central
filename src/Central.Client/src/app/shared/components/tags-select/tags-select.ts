import { Component, EventEmitter, inject, input, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TagsState } from '@core';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-tags-select',
  standalone: true,
  imports: [MtxSelectModule, MatFormFieldModule, FormsModule],
  template: `
    <mat-form-field appearance="outline" class="w-full">
      @if (!hideLabel()) {
        <mat-label>Select Tags</mat-label>
      }
      <mtx-select
        [items]="tags()"
        placeholder="Select Tags"
        multiple="true"
        bindLabel="name"
        bindValue="id",
        [ngModel]="selectedTags"
        (ngModelChange)="onSelectedTagsChange($event)"
      />
    </mat-form-field>
  `,
})
export class TagsSelect {
  private readonly store = inject(Store);
  protected tags = this.store.selectSignal(TagsState.tags);
  hideLabel = input<boolean>(false);

  @Input()
  selectedTags: number[] = [];
  @Output()
  selectedTagsChange = new EventEmitter<number[]>();

  onSelectedTagsChange(selected: number[]) {
    this.selectedTags = selected;
    this.selectedTagsChange.emit(this.selectedTags);
  }
}
