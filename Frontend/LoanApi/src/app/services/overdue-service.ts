import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class OverdueService {
  http = inject(HttpClient);
  overdueReporturl = "https://localhost:7277/api/OverdueReport/fetchOverdueReport";
  fetchOverdueReport(filters?: any) {
    return this.http.get<any>(this.overdueReporturl, { params: filters || {} });
  }
}
