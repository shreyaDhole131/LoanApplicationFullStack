import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class DisbursementReportService {
  http = inject(HttpClient);

  disburseReporturl = "https://localhost:7277/api/DisburseReport/fetchDisburseReport";
  fetchDDisburseReport(filters?: any) {
    return this.http.get<any>(this.disburseReporturl, { params: filters || {} });
  }
}
