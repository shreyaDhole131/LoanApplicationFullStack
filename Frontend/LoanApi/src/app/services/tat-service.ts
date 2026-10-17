import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class TatService {
  http = inject(HttpClient);

  tatserviceurl = "https://localhost:7277/api/TatReport/fetchTatReport";
  fetchTatReport(filters?: any) {
    return this.http.get<any>(this.tatserviceurl, { params: filters || {} });
  }
}
