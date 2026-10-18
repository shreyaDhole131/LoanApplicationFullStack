import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { CibilReportService } from '../services/cibil-reportService';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../officer/sidebar/sidebar';

@Component({
  imports: [CommonModule, FormsModule, Sidebar],
  selector: 'app-cibil-report',
  styleUrl: './cibil-report.css',
  templateUrl: './cibil-report.html',
})
export class CibilReport {

  constructor(private service: CibilReportService, private cdr: ChangeDetectorRef) {
    this.fetchCibilReport();
  }


  cibilReport: any;

  fetchCibilReport() {
    this.service.fetchCibilReport().subscribe({
      next: res => {
        console.log(res);
        this.cibilReport = res;
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

    this.service.fetchCibilReport(filters).subscribe({
      next: res => {
        this.cibilReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchCibilReport();
  }

}
