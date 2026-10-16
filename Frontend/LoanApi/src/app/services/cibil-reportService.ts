import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class CibilReportService {
  http = inject(HttpClient)

  cibilReporturl = "https://localhost:7277/api/CibilReport/fetchCibilReports"
  fetchCibilReport(filters?: any) {
    return this.http.get<any>(this.cibilReporturl, { params: filters || {} });
  }
}
