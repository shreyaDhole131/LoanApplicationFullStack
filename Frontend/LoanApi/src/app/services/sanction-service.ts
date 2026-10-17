import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SanctionService {
  http = inject(HttpClient)

  url = 'https://localhost:7277/api/SanctionLetter/pending-customers';

  fetchPendingCustomers() {
    return this.http.get<any>(this.url);
  }

  prefillurl = 'https://localhost:7277/api/SanctionLetter/prefill'
  prefillCusts(dealId: any) {
    return this.http.get<any>(`${this.prefillurl}/${dealId}`);
  }

  sanctionletterurl = 'https://localhost:7277/api/SanctionLetter/create'
  AddSanctionLetter(data: any) {
    return this.http.post<any>(this.sanctionletterurl, data);
  }

  sanctiongridurl = 'https://localhost:7277/api/SanctionLetter/grid'
  GetSanctionGrid(filters?: any) {
    return this.http.get<any>(this.sanctiongridurl, { params: filters || {} });
  }

  prefildisburseurl = "https://localhost:7277/api/SanctionLetter/fetchDisburse"
  PrefilDisbursement(dealId: number) {
    return this.http.get<any>(`${this.prefildisburseurl}`, { params: { dealId } });
  }

  applydisburseurl = "https://localhost:7277/api/SanctionLetter/PostDisburse"
  ApplyDisbursement(data: any) {
    return this.http.post<any>(this.applydisburseurl, data);
  }

  getSanctionletter = "https://localhost:7277/api/SanctionLetter"
  getSanctionLetter(letterId: number) {
    return this.http.get<any>(`${this.getSanctionLetter}/${letterId}`);
  }

  viewPdfUrl = 'https://localhost:7277/api/SanctionLetter/ViewPdf';
  downloadPdfUrl = 'https://localhost:7277/api/SanctionLetter/DownloadPdf';

  viewPdf(letterId: number): string {
    return `${this.viewPdfUrl}/${letterId}`;
  }

  downloadPdf(letterId: number): string {
    return `${this.downloadPdfUrl}/${letterId}`;
  }
}
