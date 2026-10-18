import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, PLATFORM_ID, Service } from '@angular/core';

@Service()
export class Emischeduleservice {
     http = inject(HttpClient);
  platformId = inject(PLATFORM_ID);

url = "https://localhost:7277/api/LoanAccount/my-loans";

  getMyLoanAccounts() {

    let token = '';

    if (isPlatformBrowser(this.platformId)) {
      token = localStorage.getItem('token') ?? '';
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<any>(
      this.url,
      { headers }
    );
  }

  url2 = "https://localhost:7277/api/EmiSchedule/MyEmiSchedule";

  fetchEmiSchedule(loanAccountId: number) {

    let token = '';

    if (isPlatformBrowser(this.platformId)) {
      token = localStorage.getItem('token') ?? '';
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<any>(
      `${this.url2}/${loanAccountId}`,
      { headers }
    );
  }
}
