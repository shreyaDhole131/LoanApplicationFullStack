
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { customer_loans } from '../services/customer-loans';

@Component({
  selector: 'app-closure-requests',
  imports: [CommonModule],
  templateUrl: './closure-requests.html',
  styleUrl: './closure-requests.css',
})
export class ClosureRequests implements OnInit {

  service = inject(customer_loans);
  router = inject(Router);


  selectedRequest: any = null;
allrequests: any[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit() {
    this.getAllRequests();
  }

  getAllRequests() {
    this.loading = true;
    this.errorMessage = '';

    this.service.getAllForeClosureRequests().subscribe({
      next: (data: any) => {
        this.allrequests = data;
        this.loading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.errorMessage = 'Unable to load foreclosure requests.';
        this.loading = false;
      }
    });
  }

  viewRequest(requestId: number) {
  this.selectedRequest = this.allrequests.find(
    (request: any) => request.requestId === requestId
  );
}
changeStatus(status: string) {
  this.service.changeForeClosureStatus(
    this.selectedRequest.requestId,
    status
  ).subscribe({
    next: () => {
      this.selectedRequest.status = status;
      this.getAllRequests();
    },
    error: (error: any) => {
      console.log(error);
      this.errorMessage = 'Unable to update request status.';
    }
  });
}
}
