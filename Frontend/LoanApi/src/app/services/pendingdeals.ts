import { HttpClient } from '@angular/common/http';
import { inject, Inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class Pendingdeals {
  http = inject(HttpClient);

  url = "https://localhost:7277/api/PendingDeals/SortingPending"
  fetchPendingDeals() {
    return this.http.get<any>(this.url);
  }

  fetchCusturl = "https://localhost:7277/api/PendingDeals/FetchCustomerProfile"
  fetchCustProfile(dealId: any) {
    return this.http.get<any>(this.fetchCusturl, { params: { dealId } });
  }

  reviewDealsurl = "https://localhost:7277/api/PendingDeals"
  reviewDeal(dealId: any, status: any) {
    return this.http.post<any>(`${this.reviewDealsurl}/${dealId}/Review`, null, { params: { status } });
  }

}
