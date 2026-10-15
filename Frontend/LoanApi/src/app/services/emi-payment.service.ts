
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class EmiPaymentService {

  http = inject(HttpClient);

  url = 'https://localhost:7277/api/EmiPayment';

  getHeaders() {
    const token = localStorage.getItem('token');

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getPaymentDetails(emiScheduleId: number) {
    return this.http.get<any>(
      `${this.url}/details/${emiScheduleId}`,
      { headers: this.getHeaders() }
    );
  }

  createPaymentOrder(emiScheduleId: number) {
    return this.http.post<any>(
      `${this.url}/create-order/${emiScheduleId}`,
      {},
      { headers: this.getHeaders() }
    );
  }

  verifyPayment(data: any) {
    return this.http.post<any>(
      `${this.url}/verify-payment`,
      data,
      { headers: this.getHeaders() }
    );
  }
}
