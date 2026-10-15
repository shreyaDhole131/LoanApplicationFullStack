import { ChangeDetectorRef, Component } from '@angular/core';
import { ScoreReportService } from '../services/score-report-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  imports: [FormsModule, CommonModule],
  selector: 'app-score-report',
  styleUrl: './score-report.scss',
  templateUrl: './score-report.html',
})
export class ScoreReport {
  constructor(private service: ScoreReportService, private cdr: ChangeDetectorRef) {
    this.fetchScoreReport();

  }

  scoreReport: any;

  fetchScoreReport() {
    this.service.fetchScoreReport().subscribe({
      next: res => {
        console.log(res);
        this.scoreReport = res;
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

    this.service.fetchScoreReport(filters).subscribe({
      next: res => {
        this.scoreReport = res;
        this.cdr.detectChanges();
      },
      error: err => console.log(err)
    });
  }
  resetFilters() {
    this.fromDate = '';
    this.toDate = '';
    this.fetchScoreReport();
  }

}
