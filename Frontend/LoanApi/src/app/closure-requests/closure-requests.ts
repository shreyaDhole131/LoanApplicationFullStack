
import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { customer_loans } from '../services/customer-loans';
import { Sidebar } from '../officer/sidebar/sidebar';

@Component({
  selector: 'app-closure-requests',
  standalone: true,
  imports: [CommonModule,Sidebar],
  templateUrl: './closure-requests.html',
  styleUrl: './closure-requests.css',
})
export class ClosureRequests implements OnInit {

  service = inject(customer_loans);
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);

  allrequests: any[] = [];
  selectedRequest: any = null;

  loading = false;
  loadingDetails = false;
  updatingStatus = false;

  errorMessage = '';
  modalErrorMessage = '';

  ngOnInit(): void {
    this.getAllRequests();
  }

  getAllRequests(): void {
    this.loading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.service.getAllForeClosureRequests().subscribe({
      next: (data: any) => {
        console.log('Officer foreclosure response:', data);

        let records: any[] = [];

        if (Array.isArray(data)) {
          records = data;
        } else if (Array.isArray(data?.$values)) {
          records = data.$values;
        } else if (Array.isArray(data?.data)) {
          records = data.data;
        } else if (Array.isArray(data?.data?.$values)) {
          records = data.data.$values;
        } else if (Array.isArray(data?.response)) {
          records = data.response;
        } else if (Array.isArray(data?.items)) {
          records = data.items;
        }

        this.allrequests = records;
        this.loading = false;

        console.log('Records assigned:', this.allrequests.length);
        console.log('Table records:', this.allrequests);

        if (records.length === 0) {
          this.errorMessage = 'No foreclosure requests were returned by the API.';
        }

        this.cdr.detectChanges();
      },

      error: (error: any) => {
        console.error('Foreclosure API error:', error);

        this.errorMessage =
          error.status === 401 || error.status === 403
            ? 'Access denied. Please sign in again.'
            : 'Unable to load foreclosure requests.';

        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  viewRequest(requestId: number | string): void {
    this.modalErrorMessage = '';

    this.selectedRequest = this.allrequests.find(
      (item: any) =>
        String(item.requestId ?? item.RequestId) === String(requestId)
    ) ?? null;

    this.loadingDetails = false;

    if (!this.selectedRequest) {
      this.modalErrorMessage = 'Request details could not be found.';
    }

    this.cdr.detectChanges();
  }

  changeStatus(status: string): void {
    const requestId =
      this.selectedRequest?.requestId ??
      this.selectedRequest?.RequestId;

    if (requestId == null || this.updatingStatus) {
      return;
    }

    this.updatingStatus = true;
    this.modalErrorMessage = '';

    this.service.changeForeClosureStatus(requestId, status).subscribe({
      next: () => {
        const request = this.allrequests.find(
          (item: any) =>
            String(item.requestId ?? item.RequestId) === String(requestId)
        );

        if (request) {
          request.status = status;
        }

        if (this.selectedRequest) {
          this.selectedRequest.status = status;
        }

        this.updatingStatus = false;
        this.cdr.detectChanges();
      },

      error: (error: any) => {
        console.error('Status update error:', error);

        this.modalErrorMessage =
          error?.error?.message ?? 'Unable to update request status.';

        this.updatingStatus = false;
        this.cdr.detectChanges();
      }
    });
  }
}
