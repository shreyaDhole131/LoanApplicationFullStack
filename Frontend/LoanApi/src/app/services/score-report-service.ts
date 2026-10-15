import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class ScoreReportService {
  http = inject(HttpClient);

  scoreReportUrl = "https://localhost:7277/api/ScoreReport/fetchScoreReport"
  fetchScoreReport(filters?: any) {
    return this.http.get<any>(this.scoreReportUrl, { params: filters || [] });
  }
}
