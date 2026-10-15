import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Applyloan {
  http = inject(HttpClient);

  private jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  url = "https://localhost:7277/api/LoanDeals/AddLoan"
  SaveLoan(data: any) {
    return this.http.post<any>(this.url, data);
  }

  previewurl = "https://localhost:7277/api/LoanDeals/Preview"
  Preview(data: any) {
    return this.http.post<any>(this.previewurl, data);
  }



}
