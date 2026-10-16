import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class Customer {

  http = inject(HttpClient);

  url = "https://localhost:7277/api/customer-dashboard";

  getDashboard() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.url,
      { headers }
    );
  }


  ProfileUrl = "https://localhost:7277/api/customer-profile";

  getProfileUrl() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.ProfileUrl,
      { headers }
    );
  }

  updateProfile(model: any) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.put<any>(
      this.ProfileUrl,
      model,
      { headers }
    );
  }


  KycUrl = "https://localhost:7277/api/kyc";


  getMyKycDocuments() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.KycUrl + "/my-documents",
      { headers }
    );
  }


  uploadKyc(
    documentType: string,
    file: File
  ) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    const formData = new FormData();

    formData.append(
      'documentType',
      documentType
    );

    formData.append(
      'file',
      file
    );

    return this.http.post<any>(
      this.KycUrl + "/upload",
      formData,
      { headers }
    );
  }


  viewKyc(documentId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get(
      this.KycUrl +
      "/" +
      documentId +
      "/view",
      {
        headers,
        responseType: 'blob'
      }
    );
  }


  deleteKyc(documentId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.delete<any>(
      this.KycUrl +
      "/" +
      documentId,
      { headers }
    );
  }


  SupportUrl = "https://localhost:7277/api/support";


  createTicket(model: any) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    const formData = new FormData();

    formData.append('loanAccountId', model.loanAccountId);
    formData.append('subject', model.subject);
    formData.append('description', model.description);

    if (model.file) {
      formData.append('file', model.file);
    }

    return this.http.post<any>(
      this.SupportUrl + "/create",
      formData,
      { headers }
    );
  }


  getMyTickets() {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.SupportUrl + "/my-tickets",
      { headers }
    );
  }


  getTicket(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get<any>(
      this.SupportUrl + "/" + ticketId,
      { headers }
    );
  }


  viewTicketFile(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.get(
      this.SupportUrl + "/" + ticketId + "/view",
      {
        headers,
        responseType: 'blob'
      }
    );
  }


  updateTicket(ticketId: number, model: any) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    const formData = new FormData();

    formData.append('loanAccountId', model.loanAccountId);
    formData.append('subject', model.subject);
    formData.append('description', model.description);

    if (model.file) {
      formData.append('file', model.file);
    }

    return this.http.put<any>(
      this.SupportUrl + "/" + ticketId,
      formData,
      { headers }
    );
  }


  deleteTicket(ticketId: number) {

    const token = localStorage.getItem('token');

    const headers = new HttpHeaders({
      Authorization: 'Bearer ' + token
    });

    return this.http.delete<any>(
      this.SupportUrl + "/" + ticketId,
      { headers }
    );
  }

}