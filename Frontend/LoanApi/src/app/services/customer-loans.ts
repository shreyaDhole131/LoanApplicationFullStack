
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CustomerLoansService {

  http = inject(HttpClient);

  baseUrl = 'https://localhost:7277/api/customer/foreclosure';

  // Get customer loan accounts
  getCustomerLoans() {
    return this.http.get<any[]>(`${this.baseUrl}/loans`);
  }

  // Get selected loan summary
  getLoanSummary(loanAccountId: number) {
    return this.http.get<any>(
      `${this.baseUrl}/summary/${loanAccountId}`
    );
  }

  // Submit foreclosure request
  submitForeclosureRequest(data: any) {
    return this.http.post<any>(this.baseUrl, data);
  }

  // Get customer's foreclosure requests
  getMyRequests() {
    return this.http.get<any[]>(`${this.baseUrl}/requests`);
  }
}
