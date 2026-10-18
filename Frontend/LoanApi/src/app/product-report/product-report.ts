import { ChangeDetectorRef, Component } from '@angular/core';
import { ProductReportService } from '../services/product-report-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../officer/sidebar/sidebar';

@Component({
  imports: [CommonModule, FormsModule, Sidebar],
  selector: 'app-product-report',
  styleUrl: './product-report.css',
  templateUrl: './product-report.html',
})
export class ProductReport {
  constructor(private service: ProductReportService, private cdr: ChangeDetectorRef) {
    this.fetchProductReport();
  }

  productReport: any;

  fetchProductReport() {
    this.service.fetchProductReport().subscribe({
      next: res => {
        console.log(res);
        this.productReport = res;
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

    this.service.fetchProductReport(filters).subscribe({
      next: res => {
        this.productReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchProductReport();
  }



}
