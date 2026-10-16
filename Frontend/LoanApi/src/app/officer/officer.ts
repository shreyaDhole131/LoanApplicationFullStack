import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class Officer {

  http = inject(HttpClient);

  customerUrl = "https://localhost:7277/api/officer/customer-profile";
  kycUrl = "https://localhost:7277/api/officer/kyc";
  supportTicketUrl = "https://localhost:7277/api/officer/support-tickets";

dashboardUrl = 'https://localhost:7277/api/officer-dashboard';
// =========================================================
// OFFICER DASHBOARD
// =========================================================
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
  // =========================================================
  // CUSTOMER
  // =========================================================

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


  // =========================================================
  // KYC
  // =========================================================

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


  // =========================================================
  // VIEW KYC DOCUMENT
  // =========================================================

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


  // =========================================================
  // APPROVE KYC
  // =========================================================

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


  // =========================================================
  // REJECT KYC
  // =========================================================

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


  // =========================================================
  // SUPPORT TICKETS
  // =========================================================

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


  // =========================================================
  // VIEW SUPPORT ATTACHMENT
  // =========================================================

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


  // =========================================================
  // RESOLVE SUPPORT TICKET
  // =========================================================

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