import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CibilService {

  private http = inject(HttpClient);

  // Backend HTTPS URL
  private baseUrl = 'https://localhost:7277/api/Cibil';

  // Generate CIBIL score
  GenerateMyCibil() {
    return this.http.post<any>(
      `${this.baseUrl}/GenerateMyCibil`,
      null,
      { headers: this.getHeaders() }
    );
  }

  // Get logged-in customer's CIBIL report
  GetMyCibil() {
    return this.http.get<any>(
      `${this.baseUrl}/MyCibil`,
      { headers: this.getHeaders() }
    );
  }

  // Attach JWT token
  private getHeaders(): HttpHeaders {
    const token =
      typeof localStorage !== 'undefined'
        ? localStorage.getItem('token')
        : null;

    return new HttpHeaders({
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    });
  }
}