
import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { customer_loans } from '../services/customer-loans';

@Component({
  selector: 'app-payment-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-history.html',
  styleUrl: './payment-history.css'
})
export class PaymentHistory {

  constructor(
    private service: customer_loans,
    private cd: ChangeDetectorRef
  ) {
    this.getLoanAccounts();
    this.getPaymentHistory();
  }

  LoanAccounts: any[] = [];

  paymentHistory: any[] = [];

  SelectedLoanAccount: number | null = null;

  SelectedMonths = 0;

  searchText = '';

  getLoanAccounts(): void {
    this.service.getLoanAccounts().subscribe({
      next: (res: any) => {
        console.log('FULL LOAN ACCOUNTS RESPONSE:', res);

        const data = res?.data ?? res?.Data ?? res;

        this.LoanAccounts = Array.isArray(data)
          ? data
          : data?.items ?? data?.Items ?? [];

        console.log('LOAN ACCOUNTS:', this.LoanAccounts);

        this.cd.detectChanges();
      },
      error: (err: any) => {
        console.error('Get loan accounts error:', err);

        alert(
          'Unable to load loan accounts. Status: ' +
          err.status + ' | ' +
          (err.error?.message ||
            err.message ||
            'Please check the API.')
        );
      }
    });
  }

  getPaymentHistory(): void {
    this.service.getPaymentHistory(
      this.SelectedLoanAccount,
      this.SelectedMonths
    ).subscribe({
      next: (res: any) => {
        console.log('FULL PAYMENT HISTORY RESPONSE:', res);

        const data = res?.data ?? res?.Data ?? res;

        this.paymentHistory = Array.isArray(data)
          ? data
          : data?.items ??
            data?.Items ??
            data?.payments ??
            data?.Payments ??
            [];

        console.log('PAYMENT HISTORY ROWS:', this.paymentHistory);

        this.cd.detectChanges();
      },
      error: (err: any) => {
        console.error('Get payment history error:', err);

        this.paymentHistory = [];

        alert(
          'Unable to load payment history. Status: ' +
          err.status + ' | ' +
          (err.error?.message ||
            err.message ||
            'Please check the API and authentication token.')
        );

        this.cd.detectChanges();
      }
    });
  }

  applyFilters(): void {
    this.getPaymentHistory();
  }

  get filteredPayments(): any[] {
    const search = this.searchText.toLowerCase().trim();

    if (!search) {
      return this.paymentHistory;
    }

    return this.paymentHistory.filter((payment: any) =>
      Object.values(payment).some(value =>
        String(value ?? '').toLowerCase().includes(search)
      )
    );
  }
}
