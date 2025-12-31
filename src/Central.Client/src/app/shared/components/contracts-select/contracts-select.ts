import { Component, EventEmitter, inject, input, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ContractsState } from '@core';
import { MtxSelectModule } from '@ng-matero/extensions/select';
import { Store } from '@ngxs/store';

@Component({
  selector: 'app-contracts-select',
  standalone: true,
  imports: [MtxSelectModule, MatFormFieldModule, FormsModule],
  template: `
    <mat-form-field appearance="outline" class="w-full">
      @if (!hideLabel()) {
        <mat-label>Select Contract</mat-label>
      }
      <mtx-select
        [items]="contracts()"
        placeholder="Select Contract"
        multiple="false"
        bindLabel="name"
        bindValue="id",
        [ngModel]="selected"
        (ngModelChange)="onSelectedContractChange($event)"
      />
    </mat-form-field>
  `,
})
export class ContractsSelect {
  private readonly store = inject(Store);
  protected contracts = this.store.selectSignal(ContractsState.contracts);

  hideLabel = input<boolean>(false);

  @Input()
  set selectedContract(value: number|null) {
    this.selected = value ?? undefined;
  }
  @Output()
  selectedContractChange = new EventEmitter<number>();

  selected?: number;

  onSelectedContractChange(selected: number) {
    this.selected = selected;
    this.selectedContractChange.emit(this.selected);
  }
}
