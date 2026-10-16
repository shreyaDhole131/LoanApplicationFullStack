import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class LoanProtfolioService {
  http = inject(HttpClient);

  loanprotfoliourl = "https://localhost:7277/api/LoanProtofolio/fetchProtofolioReport"
  fetchProtfolioReport(filters?: any) {
    return this.http.get<any>(this.loanprotfoliourl, { params: filters || {} });
  }
}
