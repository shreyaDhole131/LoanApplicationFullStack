import { ChangeDetectorRef, Component } from '@angular/core';
import { Pendingdeals } from '../services/pendingdeals';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-pending-loans',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './pending-loans.html',
  styleUrl: './pending-loans.scss',
})
export class PendingLoans {
  constructor(private service: Pendingdeals, private cdr: ChangeDetectorRef) {
    this.getAllPendingDeals();
  }
  pendingdeals: any;

  loanType = '';
  minAmount: any = '';
  maxAmount: any = '';
  dateFilter = '';
  fromDate: any = '';
  toDate: any = '';
  search: any = '';

  getAllPendingDeals() {
    this.service.fetchPendingDeals().subscribe({
      next: res => {
        console.log(res);
        this.pendingdeals = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }

  applyFilters() {
    this.page = 1;
    const filters: any = {};
    if (this.loanType) filters.LoanType = this.loanType;
    if (this.minAmount) filters.MinAmount = this.minAmount;
    if (this.maxAmount) filters.MaxAmount = this.maxAmount;
    if (this.dateFilter) filters.dateFilter = this.dateFilter;
    if (this.fromDate) filters.FromFilter = this.fromDate;
    if (this.toDate) filters.ToFilter = this.toDate;
    if (this.search) filters.search = this.search;

    this.service.fetchPendingDeals(filters).subscribe({
      next: res => {
        this.pendingdeals = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.page = 1;
    this.pageSize = 10;
    this.loanType = '';
    this.minAmount = '';
    this.maxAmount = '';
    this.dateFilter = '';
    this.fromDate = '';
    this.toDate = '';
    this.search = '';
    this.getAllPendingDeals();
  }


  // pagination
  page = 1;
  pageSize = 10;

  get pagedDeals(): any[] {
    const start = (this.page - 1) * this.pageSize;
    return (this.pendingdeals || []).slice(start, start + this.pageSize);
  }

  get totalCount(): number {
    return this.pendingdeals?.length || 0;
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize) || 1;
  }

  get pagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goToPage(n: number) {
    if (n < 1 || n > this.totalPages) return;
    this.page = n;
  }

  onPageSizeChange() {
    this.page = 1;
  }



}
