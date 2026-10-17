import { ChangeDetectorRef, Component } from '@angular/core';
import { OverdueService } from '../services/overdue-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule],
  selector: 'app-overdue-report',
  styleUrl: './overdue-report.css',
  templateUrl: './overdue-report.html',
})
export class OverdueReport {
  constructor(private service: OverdueService, private cdr: ChangeDetectorRef) {
    this.fetchOverdueReport();
  }

  overdueReport: any;
  fetchOverdueReport() {
    this.service.fetchOverdueReport().subscribe({
      next: res => {
        console.log(res);
        this.overdueReport = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }

    });
  }

  onDate: any = '';
  applyfilters() {
    const filters: any = {};
    if (this.onDate) filters.onDate = this.onDate;

    this.service.fetchOverdueReport(filters).subscribe({
      next: res => {
        this.overdueReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.onDate = '';
    this.fetchOverdueReport();
  }

}
