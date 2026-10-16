import { ChangeDetectorRef, Component } from '@angular/core';
import { EmiReportService } from '../services/emi-report-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule],
  selector: 'app-emi-report',
  styleUrl: './emi-report.scss',
  templateUrl: './emi-report.html',
})
export class EmiReport {
  constructor(private service: EmiReportService, private cdr: ChangeDetectorRef) {
    this.fetchemiReport();
  }


  emiReport: any;

  fetchemiReport() {
    this.service.fetchEmiReport().subscribe({
      next: res => {
        console.log(res);
        this.emiReport = res;
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

    this.service.fetchEmiReport(filters).subscribe({
      next: res => {
        this.emiReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchemiReport();
  }
}
