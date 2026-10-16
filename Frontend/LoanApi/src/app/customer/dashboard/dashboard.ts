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

  // Customer service
  customer = inject(Customer);

  // Dashboard API data
  data: any = null;

  // Loading state
  loading = true;

  // Error message
  errorMessage = '';


  // ==========================================
  // CONSTRUCTOR
  // ==========================================

  constructor() {
    this.getDashboard();
  }


  // ==========================================
  // GET CUSTOMER DASHBOARD
  // ==========================================

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


  // ==========================================
  // FORMAT MONEY
  // ==========================================

  formatAmount(
    amount: number | null | undefined
  ): string {

    if (amount == null) {
      return '₹0';
    }

    return '₹' +
      Number(amount).toLocaleString('en-IN');
  }


  // ==========================================
  // FORMAT DATE
  // ==========================================

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


  // ==========================================
  // EMI PAID PERCENTAGE
  // ==========================================

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


  // ==========================================
  // EMI REMAINING PERCENTAGE
  // ==========================================

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


  // ==========================================
  // APPLICATION PROGRESS
  // ==========================================

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


    // Active loan means complete journey
    if (loanStatus === 'active') {
      return 100;
    }


    // Approved application
    if (status === 'approved') {
      return 75;
    }


    // Processing application
    if (status === 'processing') {
      return 50;
    }


    // Pending application
    if (status === 'pending') {
      return 25;
    }


    // Submitted application
    if (status === 'submitted') {
      return 25;
    }


    return 0;
  }


  // ==========================================
  // CHECK APPLICATION STEP
  // ==========================================

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


    // Submitted
    if (step === 'submitted') {
      return true;
    }


    // Approved
    if (step === 'approved') {

      return (
        status === 'approved' ||
        loanStatus === 'active'
      );
    }


    // Disbursed
    if (step === 'disbursed') {

      return loanStatus === 'active';
    }


    // Active
    if (step === 'active') {

      return loanStatus === 'active';
    }


    return false;
  }


  // ==========================================
  // STATUS CSS CLASS
  // ==========================================

  getStatusClass(status: string): string {

    if (!status) {
      return 'status-default';
    }

    const value =
      status.toLowerCase();


    // SUCCESS
    if (
      value === 'active' ||
      value === 'approved' ||
      value === 'paid'
    ) {

      return 'status-success';
    }


    // WARNING
    if (
      value === 'pending' ||
      value === 'processing'
    ) {

      return 'status-warning';
    }


    // DANGER
    if (
      value === 'rejected' ||
      value === 'failed'
    ) {

      return 'status-danger';
    }


    // DEFAULT
    return 'status-default';
  }

}