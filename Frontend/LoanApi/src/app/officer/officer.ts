import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class Officer {

  http = inject(HttpClient);

  customerUrl = "https://localhost:7277/api/officer/customer-profile";
  kycUrl = "https://localhost:7277/api/officer/kyc";
  supportTicketUrl = "https://localhost:7277/api/officer/support-tickets";

dashboardUrl = 'https://localhost:7277/api/officer-dashboard';
getOfficerDashboard() {
  const token = localStorage.getItem('token');

  const headers = new HttpHeaders({
    Authorization: 'Bearer ' + token
  });

  return this.http.get<any>(
    this.dashboardUrl,
    { headers }
  );
}

  getAllCustomers() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any[]>(
      this.customerUrl,
      { headers }
    );
  }

  getCustomerProfile(customerId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.customerUrl + '/' + customerId,
      { headers }
    );
  }

  getKycCustomers() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any[]>(
      this.kycUrl,
      { headers }
    );
  }

  getCustomerKyc(customerId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.kycUrl + '/' + customerId,
      { headers }
    );
  }

  viewKycDocument(documentId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get(
      this.kycUrl + '/document/' + documentId + '/view',
      {
        headers: headers,
        responseType: 'blob'
      }
    );
  }

  approveKyc(documentIds: number[]) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.put(
      this.kycUrl + '/approve',
      {
        documentIds: documentIds
      },
      {
        headers: headers
      }
    );
  }

  rejectKyc(documentIds: number[]) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.put(
      this.kycUrl + '/reject',
      {
        documentIds: documentIds
      },
      {
        headers: headers
      }
    );
  }

  getAllSupportTickets() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any[]>(
      this.supportTicketUrl,
      { headers }
    );
  }

  getSupportTicket(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.supportTicketUrl + '/' + ticketId,
      { headers }
    );
  }

  viewSupportAttachment(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get(
      this.supportTicketUrl + '/' + ticketId + '/view',
      {
        headers: headers,
        responseType: 'blob'
      }
    );
  }

  resolveSupportTicket(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.put(
      this.supportTicketUrl + '/' + ticketId + '/resolve',
      {},
      { headers }
    );
  }

}