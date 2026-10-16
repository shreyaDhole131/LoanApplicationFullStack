import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class PrepaymentService {
  http = inject(HttpClient);

  prepaymenturl = "https://localhost:7277/api/PrePayment/fetchPrepayment";
  fetchPrepaymentReport(filters?: any) {
    return this.http.get<any>(this.prepaymenturl, { params: filters || {} });
  }
}
