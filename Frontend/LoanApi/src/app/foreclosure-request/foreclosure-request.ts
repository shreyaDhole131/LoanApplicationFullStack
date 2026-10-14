
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerLoansService } from '../services/customer-loans';
@Component({
  selector: 'app-foreclosure-request',
  imports: [CommonModule, FormsModule],
  templateUrl: './foreclosure-request.html',
  styleUrl: './foreclosure-request.css'
})
export class ForeclosureRequest implements OnInit {

  private service = inject(CustomerLoansService);

  loans: any[] = [];
  requests: any[] = [];
  summary: any = null;

  loanAccountId: number | null = null;
  foreclosureType = 'Full';
  partialAmount: number | null = null;
  expectedClosureDate = new Date().toISOString().substring(0, 10);
  reason = '';

  message = '';
  errorMessage = '';

  ngOnInit(): void {
    this.loadLoans();
    this.loadRequests();
  }

  loadLoans(): void {
    this.service.getCustomerLoans().subscribe({
      next: (res) => {
        this.loans = res;
      },
      error: (err) => {
        this.errorMessage = 'Unable to load loan accounts.';
        console.error(err);
      }
    });
  }

  onLoanChange(): void {
    this.summary = null;
    this.message = '';
    this.errorMessage = '';

    if (this.loanAccountId === null) {
      return;
    }

    this.service.getLoanSummary(this.loanAccountId).subscribe({
      next: (res) => {
        this.summary = res;
      },
      error: (err) => {
        this.errorMessage = 'Unable to load loan summary.';
        console.error(err);
      }
    });
  }

  loadRequests(): void {
    this.service.getMyRequests().subscribe({
      next: (res) => {
        this.requests = res;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  submitRequest(): void {
    this.message = '';
    this.errorMessage = '';

    if (this.loanAccountId === null) {
      this.errorMessage = 'Please select a loan account.';
      return;
    }

    if (!this.expectedClosureDate) {
      this.errorMessage = 'Please select an expected closure date.';
      return;
    }

    if (this.expectedClosureDate < new Date().toISOString().substring(0, 10)) {
      this.errorMessage = 'Closure date cannot be in the past.';
      return;
    }

    if (this.foreclosureType === 'Partial') {
      if (
        this.partialAmount === null ||
        this.partialAmount <= 0 ||
        this.partialAmount >= Number(this.summary?.outstandingPrincipal)
      ) {
        this.errorMessage =
          'Partial amount must be greater than zero and less than the outstanding principal.';
        return;
      }
    }

    const data = {
      loanAccountId: this.loanAccountId,
      foreClosureType: this.foreclosureType,
      partialAmount:
        this.foreclosureType === 'Partial' ? this.partialAmount : 0,
      expectedClosureDate: this.expectedClosureDate,
      reason: this.reason
    };

    this.service.submitForeclosureRequest(data).subscribe({
      next: () => {
        this.message = 'Foreclosure request submitted successfully.';
        this.partialAmount = null;
        this.reason = '';
        this.loadRequests();
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || 'Unable to submit foreclosure request.';
        console.error(err);
      }
    });
  }
}
