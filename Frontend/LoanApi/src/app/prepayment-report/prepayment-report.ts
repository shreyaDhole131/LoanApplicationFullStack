import { ChangeDetectorRef, Component } from '@angular/core';
import { PrepaymentService } from '../services/prepayment-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../officer/sidebar/sidebar';

@Component({
  imports: [CommonModule, FormsModule, Sidebar],
  selector: 'app-prepayment-report',
  styleUrl: './prepayment-report.css',
  templateUrl: './prepayment-report.html',
})
export class PrepaymentReport {
  constructor(private service: PrepaymentService, private cdr: ChangeDetectorRef) {
    this.fetchPrepaymentReport();
  }


  prepaymentReport: any;


  fetchPrepaymentReport() {
    this.service.fetchPrepaymentReport().subscribe({
      next: res => {
        console.log(res);
        this.prepaymentReport = res;
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

    this.service.fetchPrepaymentReport(filters).subscribe({
      next: res => {
        this.prepaymentReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchPrepaymentReport();
  }
}
