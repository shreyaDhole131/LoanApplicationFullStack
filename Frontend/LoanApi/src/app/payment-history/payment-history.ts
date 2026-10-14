
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerLoansService } from '../services/customer-loans';

@Component({
  selector: 'app-payment-history',
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-history.html',
  styleUrl: './payment-history.css'
})
export class PaymentHistory implements OnInit {

  private service = inject(CustomerLoansService);

  loans: any[] = [];
  payments: any[] = [];

  loanAccountId: number | null = null;
  period = '1M';
  searchText = '';
  pageSize = 10;
  currentPage = 1;

  sortColumn = 'paymentDate';
  sortDirection = 'desc';

  ngOnInit() {
    this.loadLoans();
  }

  loadLoans() {
    this.service.getCustomerLoans().subscribe({
      next: (res) => {
        this.loans = res;
      },
      error: (err) => console.error(err)
    });
  }

  onFilterChange() {
    this.currentPage = 1;
    this.loadPayments();
  }

  loadPayments() {
    if (this.loanAccountId === null) {
      this.payments = [];
      return;
    }

    this.service.getPaymentHistory(this.loanAccountId, this.period)
      .subscribe({
        next: (res) => {
          this.payments = res;
          this.currentPage = 1;
        },
        error: (err) => {
          console.error(err);
          this.payments = [];
        }
      });
  }

  get filteredPayments(): any[] {
    const search = this.searchText.toLowerCase().trim();

    return this.payments
      .filter(payment =>
        Object.values(payment).some(value =>
          String(value ?? '').toLowerCase().includes(search)
        )
      )
      .sort((a, b) => {
        const first = a[this.sortColumn] ?? '';
        const second = b[this.sortColumn] ?? '';

        let result: number;

        if (this.sortColumn === 'amount') {
          result = Number(first) - Number(second);
        } else {
          result = String(first).localeCompare(String(second));
        }

        return this.sortDirection === 'asc' ? result : -result;
      });
  }

  get paginatedPayments(): any[] {
    const start = (this.currentPage - 1) * this.pageSize;

    return this.filteredPayments.slice(start, start + this.pageSize);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredPayments.length / this.pageSize);
  }

  get startEntry(): number {
    return this.filteredPayments.length === 0
      ? 0
      : (this.currentPage - 1) * this.pageSize + 1;
  }

  get endEntry(): number {
    return Math.min(
      this.currentPage * this.pageSize,
      this.filteredPayments.length
    );
  }

  sortBy(column: string) {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
  }

  changePage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  onSearchChange() {
    this.currentPage = 1;
  }

  onPageSizeChange() {
    this.currentPage = 1;
  }
}
