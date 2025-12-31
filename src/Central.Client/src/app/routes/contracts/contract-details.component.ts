import { Component, OnInit, inject, input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { PageHeader } from '@shared';
import { ContractsState, ContractsActions } from '@core';
import { Contract } from '../../models/contract.model';

@Component({
  selector: 'app-contract-details',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatChipsModule,
    TranslateModule,
    PageHeader,
  ],
  template: `
    <page-header />
    <mat-card>
      <mat-card-header>
        <mat-card-title>{{ contract()?.name }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        @if (loading()) {
          <div class="loading">{{ 'common.loading' | translate }}</div>
        } @else if (contract()) {
          <div class="details">
            <div class="detail-row">
              <strong>{{ 'contracts.state' | translate }}</strong>
              <mat-chip [class]="'state-' + contract()!.state.toLowerCase()">
                {{ 'contracts.states.' + contract()!.state | translate }}
              </mat-chip>
            </div>

            <div class="detail-row">
              <strong>{{ 'contracts.description' | translate }}</strong>
              <span>{{ contract()!.description || '-' }}</span>
            </div>

            <div class="detail-row">
              <strong>{{ 'contracts.customerId' | translate }}</strong>
              <span>{{ contract()!.customerId || '-' }}</span>
            </div>

            <div class="detail-row">
              <strong>{{ 'contracts.contractId' | translate }}</strong>
              <span>{{ contract()!.contractId || '-' }}</span>
            </div>

            <div class="detail-row">
              <strong>{{ 'contracts.created' | translate }}</strong>
              <span>{{ contract()!.created | date: 'short' }}</span>
            </div>

            <div class="detail-row">
              <strong>{{ 'contracts.updated' | translate }}</strong>
              <span>{{ contract()!.updated | date: 'short' }}</span>
            </div>
          </div>
        } @else {
          <div class="no-data">
            <p>{{ 'contracts.notFound' | translate }}</p>
          </div>
        }
      </mat-card-content>
      <mat-card-actions>
        <button mat-button (click)="goBack()">
          <mat-icon>arrow_back</mat-icon>
          {{ 'common.back' | translate }}
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .details {
      display: flex;
      flex-direction: column;
      gap: 16px;
      padding: 16px 0;
    }

    .detail-row {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .detail-row label {
      font-weight: 500;
      color: rgba(0, 0, 0, 0.6);
      font-size: 14px;
    }

    .detail-row span {
      font-size: 16px;
    }

    .loading,
    .no-data {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 40px 20px;
      text-align: center;
    }

    mat-chip.state-draft {
      background-color: #9e9e9e;
      color: white;
    }

    mat-chip.state-active {
      background-color: #4caf50;
      color: white;
    }

    mat-chip.state-expired {
      background-color: #ff9800;
      color: white;
    }

    mat-chip.state-terminated {
      background-color: #f44336;
      color: white;
    }
  `],
})
export class ContractDetailsComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly router = inject(Router);

  id = input.required<number>();
  contracts = this.store.selectSignal(ContractsState.contracts);
  loading = this.store.selectSignal(ContractsState.loading);

  contract = signal<Contract | undefined>(undefined);

  ngOnInit() {
    this.store.dispatch(new ContractsActions.Load());
    
    // Watch for contracts to load and find the one we need
    const contractId = this.id();
    const allContracts = this.contracts();
    const foundContract = allContracts.find(c => c.id === contractId);
    if (foundContract) {
      this.contract.set(foundContract);
    }
  }

  goBack() {
    this.router.navigate(['/contracts']);
  }
}
