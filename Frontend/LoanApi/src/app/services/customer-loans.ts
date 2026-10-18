
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class customer_loans {

  http = inject(HttpClient);

  // Customer foreclosure requests
  url = 'https://localhost:7277/api/customer/foreclosure/loans';

  getLoanAccounts() {
    return this.http.get<any>(this.url);
  }

  url2 = 'https://localhost:7277/api/customer/foreclosure/summary';

  getCardsData(loanAccountId: number) {
    return this.http.get<any>(`${this.url2}/${loanAccountId}`);
  }

  url3 = 'https://localhost:7277/api/customer/foreclosure';

  AddForeClosure(data: any) {
    return this.http.post<any>(this.url3, data);
  }

  url4 = 'https://localhost:7277/api/customer/foreclosure/requests';

  getForeClosureRequests() {
    return this.http.get<any>(this.url4);
  }

  url5 = 'https://localhost:7277/api/customer/foreclosure/payment/create-order';

  createPaymentOrder(requestId: number) {
    return this.http.post<any>(`${this.url5}/${requestId}`, {});
  }

  url6 = 'https://localhost:7277/api/customer/foreclosure/payment';

  verifyPayment(data: {
    requestId: number;
    razorpayOrderId: string;
    razorpayPaymentId: string;
    razorpaySignature: string;
  }) {
    return this.http.post<any>(`${this.url6}/verify`, data);
  }

  // Customer payment history
  url7 = 'https://localhost:7277/api/customer/payment-history';

  getPaymentHistory(loanAccountId: number | null, months: number) {
    let requestUrl = `${this.url7}?months=${months}`;

    if (loanAccountId !== null) {
      requestUrl += `&loanAccountId=${loanAccountId}`;
    }

    return this.http.get<any>(requestUrl);
  }

  // Officer foreclosure requests
  url8 = 'https://localhost:7277/api/officer/foreclosure';

  getAllForeClosureRequests() {
    return this.http.get<any>(this.url8);
  }

  getForeClosureRequestById(requestId: number) {
    return this.http.get<any>(`${this.url8}/${requestId}`);
  }

  changeForeClosureStatus(requestId: number, status: string) {
    // The backend controller accepts a JSON string, e.g. "Approved".
    return this.http.put<any>(
      `${this.url8}/${requestId}`,
      status
    );
  }
}
