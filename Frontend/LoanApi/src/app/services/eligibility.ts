import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EligibilityService {

  private http = inject(HttpClient);

  // Backend API base URL
  private url = 'https://localhost:7277/api/Eligibility';

  // Attach JWT token
  private getHeaders(): HttpHeaders {
    const token =
      typeof localStorage !== 'undefined'
        ? localStorage.getItem('token')
        : null;

    return new HttpHeaders(
      token
        ? { Authorization: `Bearer ${token}` }
        : {}
    );
  }

  // Process automatic eligibility
  ProcessAutomaticEligibility() {
    return this.http.post<any>(
      `${this.url}/process-automatic`,
      null,
      { headers: this.getHeaders() }
    );
  }

  // Get customers awaiting manual review
  GetManualReviewCustomers() {
    return this.http.get<any>(
      `${this.url}/manual-review-customers`,
      { headers: this.getHeaders() }
    );
  }

  // Evaluate a customer's eligibility
  EvaluateEligibility(customerId: number) {
    return this.http.post<any>(
      `${this.url}/evaluate/${customerId}`,
      null,
      { headers: this.getHeaders() }
    );
  }

  // Approve eligibility
  ApproveEligibility(customerId: number) {
    return this.http.post<any>(
      `${this.url}/approve/${customerId}`,
      null,
      { headers: this.getHeaders() }
    );
  }

  // Reject eligibility
  RejectEligibility(
    customerId: number,
    rejectionReason: string,
    otherReason: string
  ) {
    return this.http.post<any>(
      `${this.url}/reject/${customerId}`,
      {
        rejectionReason,
        otherReason
      },
      { headers: this.getHeaders() }
    );
  }

  // Get the logged-in customer's eligibility result
  GetMyResult() {
    return this.http.get<any>(
      `${this.url}/my-result`,
      { headers: this.getHeaders() }
    );
  }
}