import { Component } from '@angular/core';
import { Officer } from '../officer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-kyc-verification',
  imports: [Sidebar],
  templateUrl: './kyc-verification.html',
  styleUrl: './kyc-verification.css'
})
export class KycVerification {

  customers: any[] = [];

  selectedCustomer: any = null;

  selectedDocuments: number[] = [];


  constructor(private service: Officer) {

    this.getKycCustomers();

  }


  // =========================================================
  // GET CUSTOMERS HAVING KYC
  // =========================================================

  getKycCustomers() {

    this.service.getKycCustomers().subscribe({

      next: (res: any[]) => {

        console.log('KYC CUSTOMERS:', res);

        this.customers = res;

      },

      error: (err: any) => {

        console.log('GET KYC CUSTOMERS ERROR:', err);

      }

    });

  }


  // =========================================================
  // VIEW CUSTOMER KYC
  // =========================================================

  viewKyc(customerId: number) {

    this.service.getCustomerKyc(customerId).subscribe({

      next: (res: any) => {

        console.log('CUSTOMER KYC:', res);

        this.selectedCustomer = res;

        this.selectedDocuments = [];

      },

      error: (err: any) => {

        console.log('GET CUSTOMER KYC ERROR:', err);

      }

    });

  }


  // =========================================================
  // VIEW DOCUMENT
  // =========================================================

  viewDocument(documentId: number) {

    console.log('VIEW KYC DOCUMENT:', documentId);

    const newTab = window.open('', '_blank');

    if (!newTab) {

      alert('Please allow pop-ups for this website.');

      return;

    }


    this.service.viewKycDocument(documentId).subscribe({

      next: (file: Blob) => {

        console.log('KYC FILE RECEIVED:', file);

        const fileUrl = URL.createObjectURL(file);

        newTab.location.href = fileUrl;


        setTimeout(() => {

          URL.revokeObjectURL(fileUrl);

        }, 60000);

      },


      error: (err: any) => {

        console.log('VIEW KYC DOCUMENT ERROR:', err);

        newTab.close();

        alert('Unable to open KYC document.');

      }

    });

  }


  // =========================================================
  // SELECT / UNSELECT DOCUMENT
  // =========================================================

  selectDocument(
    documentId: number,
    event: any
  ) {

    if (event.target.checked) {

      if (!this.selectedDocuments.includes(documentId)) {

        this.selectedDocuments.push(documentId);

      }

    }
    else {

      this.selectedDocuments =
        this.selectedDocuments.filter(
          id => id !== documentId
        );

    }

    console.log(
      'SELECTED DOCUMENTS:',
      this.selectedDocuments
    );

  }


  // =========================================================
  // APPROVE SELECTED DOCUMENTS
  // =========================================================

  approve() {

    if (this.selectedDocuments.length === 0) {

      alert('Please select at least one document');

      return;

    }


    this.service
      .approveKyc(this.selectedDocuments)
      .subscribe({

        next: (res: any) => {

          console.log('APPROVE RESPONSE:', res);

          alert('KYC approved successfully');

          this.selectedDocuments = [];

          this.viewKyc(
            this.selectedCustomer.customerId
          );

        },


        error: (err: any) => {

          console.log('APPROVE KYC ERROR:', err);

          alert('Unable to approve KYC');

        }

      });

  }


  // =========================================================
  // REJECT SELECTED DOCUMENTS
  // =========================================================

  reject() {

    if (this.selectedDocuments.length === 0) {

      alert('Please select at least one document');

      return;

    }


    this.service
      .rejectKyc(this.selectedDocuments)
      .subscribe({

        next: (res: any) => {

          console.log('REJECT RESPONSE:', res);

          alert('KYC rejected successfully');

          this.selectedDocuments = [];

          this.viewKyc(
            this.selectedCustomer.customerId
          );

        },


        error: (err: any) => {

          console.log('REJECT KYC ERROR:', err);

          alert('Unable to reject KYC');

        }

      });

  }


  // =========================================================
  // BACK TO CUSTOMER LIST
  // =========================================================

  backToCustomers() {

    this.selectedCustomer = null;

    this.selectedDocuments = [];

  }

}