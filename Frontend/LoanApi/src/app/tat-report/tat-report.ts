import { ChangeDetectorRef, Component } from '@angular/core';
import { TatService } from '../services/tat-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule],
  selector: 'app-tat-report',
  styleUrl: './tat-report.css',
  templateUrl: './tat-report.html',
})
export class TatReport {
  constructor(private service: TatService, private cdr: ChangeDetectorRef) {
    this.fetchTatReport();

  }

  tatReport: any;
  fetchTatReport() {
    this.service.fetchTatReport().subscribe({
      next: res => {
        console.log(res);
        this.tatReport = res;
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
    this.service.fetchTatReport(filters).subscribe({
      next: res => {
        console.log(res);
        this.tatReport = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }
  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchTatReport();
  }

}
