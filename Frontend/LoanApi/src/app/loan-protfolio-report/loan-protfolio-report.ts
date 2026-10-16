import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { LoanProtfolioService } from '../services/loan-protfolio-service';
import { FormsModule } from '@angular/forms';

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-loan-protfolio-report',
  styleUrl: './loan-protfolio-report.css',
  templateUrl: './loan-protfolio-report.html',
})
export class LoanProtfolioReport {
  constructor(private service: LoanProtfolioService, private cdr: ChangeDetectorRef) {
    this.fetchProtfolioReport();
  }


  protfolioReport: any;

  fetchProtfolioReport() {
    this.service.fetchProtfolioReport().subscribe({
      next: res => {
        console.log(res);
        this.protfolioReport = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }

  fromDate: any = '';
  toDate: any = '';

  applyfilters() {
    const filters: any = {};
    if (this.fromDate) filters.fromDate = this.fromDate;
    if (this.toDate) filters.toDate = this.toDate;

    this.service.fetchProtfolioReport(filters).subscribe({
      next: res => {
        this.protfolioReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchProtfolioReport();
  }

}
