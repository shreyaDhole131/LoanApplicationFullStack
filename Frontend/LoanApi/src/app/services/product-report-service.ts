import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class ProductReportService {
  http = inject(HttpClient);

  productwiseurl = "https://localhost:7277/api/ProductReport/fetchProductReport"
  fetchProductReport(filters?: any) {
    return this.http.get<any>(this.productwiseurl, { params: filters || {} });
  }
}
