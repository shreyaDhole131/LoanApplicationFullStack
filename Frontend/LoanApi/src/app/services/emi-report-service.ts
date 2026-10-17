import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class EmiReportService {
  http = inject(HttpClient);

  emiurl = "https://localhost:7277/api/EmiReport/fetchEmiReport";
  fetchEmiReport(filters?: any) {
    return this.http.get<any>(this.emiurl, { params: filters || {} });
  }
}
