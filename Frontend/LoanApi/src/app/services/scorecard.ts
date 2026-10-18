import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ScorecardService {

  private http = inject(HttpClient);

  private baseUrl = 'https://localhost:7277/api/ScoreCard';

  // Get customers for the officer's ScoreCard page
  GetCustomers() {
    return this.http.get<any>(
      `${this.baseUrl}/customers`,
      { headers: this.getHeaders() }
    );
  }

  // Generate a ScoreCard for the selected customer
  GenerateScoreCard(customerId: number) {
    return this.http.post<any>(
      `${this.baseUrl}/Generate/${customerId}`,
      null,
      { headers: this.getHeaders() }
    );
  }

  // Attach the officer's JWT token
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
}