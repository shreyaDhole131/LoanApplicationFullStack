import { ChangeDetectorRef, Component } from '@angular/core';
import { DisbursementReportService } from '../services/disbursement-report-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule],
  selector: 'app-disburse-report',
  styleUrl: './disburse-report.css',
  templateUrl: './disburse-report.html',
})
export class DisburseReport {
  constructor(private service: DisbursementReportService, private cdr: ChangeDetectorRef) {
    this.fetchDisburseReport();

  }


  disburseReport: any;

  fetchDisburseReport() {
    this.service.fetchDDisburseReport().subscribe({
      next: res => {
        console.log(res);
        this.disburseReport = res;
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

    this.service.fetchDDisburseReport(filters).subscribe({
      next: res => {
        this.disburseReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchDisburseReport();
  }

}
