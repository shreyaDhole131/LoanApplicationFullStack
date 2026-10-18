import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Customer } from '../customer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, Sidebar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard {

  customer = inject(Customer);

  data: any = null;

  loading = true;

  errorMessage = '';

  constructor() {
    this.getDashboard();
  }

  getDashboard() {

    this.loading = true;
    this.errorMessage = '';

    this.customer.getDashboard().subscribe({

      next: (res: any) => {

        console.log('DASHBOARD RESPONSE:', res);

        this.data = res;

        this.loading = false;
      },

      error: (err: any) => {

        console.log('DASHBOARD ERROR:', err);

        this.loading = false;

        this.errorMessage =
          'Unable to load dashboard data.';
      }

    });
  }

  formatAmount(
    amount: number | null | undefined
  ): string {

    if (amount == null) {
      return '₹0';
    }

    return '₹' +
      Number(amount).toLocaleString('en-IN');
  }

  formatDate(
    date: string | null | undefined
  ): string {

    if (!date) {
      return '-';
    }

    return new Date(date).toLocaleDateString(
      'en-IN',
      {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
      }
    );
  }

  getEmiPercentage(): number {

    const total =
      this.data?.emiSummary?.totalEmis || 0;

    const paid =
      this.data?.emiSummary?.paidEmis || 0;

    if (total === 0) {
      return 0;
    }

    return Math.round(
      (paid / total) * 100
    );
  }

  getRemainingPercentage(): number {

    const total =
      this.data?.emiSummary?.totalEmis || 0;

    const remaining =
      this.data?.emiSummary?.remainingEmis || 0;

    if (total === 0) {
      return 0;
    }

    return Math.round(
      (remaining / total) * 100
    );
  }

  getApplicationProgress(): number {

    const application =
      this.data?.applications?.[0];

    if (!application) {
      return 0;
    }

    const status =
      application.currentStatus?.toLowerCase();

    const loanStatus =
      this.data?.selectedLoanDetails
        ?.loanStatus?.toLowerCase();

    if (loanStatus === 'active') {
      return 100;
    }

    if (status === 'approved') {
      return 75;
    }

    if (status === 'processing') {
      return 50;
    }

    if (status === 'pending') {
      return 25;
    }

    if (status === 'submitted') {
      return 25;
    }

    return 0;
  }

  isStepCompleted(step: string): boolean {

    const application =
      this.data?.applications?.[0];

    if (!application) {
      return false;
    }

    const status =
      application.currentStatus?.toLowerCase();

    const loanStatus =
      this.data?.selectedLoanDetails
        ?.loanStatus?.toLowerCase();

    if (step === 'submitted') {
      return true;
    }

    if (step === 'approved') {

      return (
        status === 'approved' ||
        loanStatus === 'active'
      );
    }

    if (step === 'disbursed') {

      return loanStatus === 'active';
    }

    if (step === 'active') {

      return loanStatus === 'active';
    }

    return false;
  }

  getStatusClass(status: string): string {

    if (!status) {
      return 'status-default';
    }

    const value =
      status.toLowerCase();

    if (
      value === 'active' ||
      value === 'approved' ||
      value === 'paid'
    ) {

      return 'status-success';
    }

    if (
      value === 'pending' ||
      value === 'processing'
    ) {

      return 'status-warning';
    }

    if (
      value === 'rejected' ||
      value === 'failed'
    ) {

      return 'status-danger';
    }

    return 'status-default';
  }

}