
import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { customer_loans } from '../services/customer-loans';
<<<<<<< HEAD
import { nextTick } from 'process';
import { Sidebar } from '../customer/sidebar/sidebar';
=======
>>>>>>> 0f6a40688c82c8dc7f57919a0bc03046a0c4532f

declare const Razorpay: any;

@Component({
  selector: 'app-foreclosure-request',
<<<<<<< HEAD
    standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule,Sidebar],
=======
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
>>>>>>> 0f6a40688c82c8dc7f57919a0bc03046a0c4532f
  templateUrl: './foreclosure-request.html',
  styleUrl: './foreclosure-request.css'
})
export class foreclosure_request {
  constructor(
    private service: customer_loans,
    private cd: ChangeDetectorRef
  ) {
    this.getLoanAccounts();
    this.getForeClosureRequests();
  }

  allrequests: any[] = [];
  payingRequestId: number | null = null;

  getForeClosureRequests(): void {
    this.service.getForeClosureRequests().subscribe({
      next: (res: any) => {
        console.log('FULL FORECLOSURE API RESPONSE:', res);

        const responseData = res?.data ?? res?.Data ?? res;

        this.allrequests = Array.isArray(responseData)
          ? responseData
          : responseData?.requests ??
            responseData?.Requests ??
            responseData?.items ??
            responseData?.Items ??
            [];

        console.log('FORECLOSURE REQUEST ROWS:', this.allrequests);

        if (this.allrequests.length > 0) {
          console.log('FIRST FORECLOSURE REQUEST:', this.allrequests[0]);
          console.log(
            'FIRST REQUEST FIELD NAMES:',
            Object.keys(this.allrequests[0])
          );
          console.log(
            'FIRST REQUEST ID:',
            this.getRequestId(this.allrequests[0])
          );
        }

        this.cd.detectChanges();
      },
      error: err => {
        console.error('Get foreclosure requests error:', err);
        alert(
          'API error. Status: ' + err.status + ' | ' +
          (err.error?.message || err.message ||
            'Unable to load foreclosure requests.')
        );
      }
    });
  }

  LoanAccounts: any[] = [];
  SelectedLoanAccount: any = null;

  getLoanAccounts(): void {
    this.service.getLoanAccounts().subscribe({
      next: (res: any) => {
        console.log('Loan accounts:', res);

        const data = res?.data ?? res?.Data ?? res;
        this.LoanAccounts = Array.isArray(data) ? data : [];

        this.cd.detectChanges();
      },
      error: err => {
        console.error('Get loan accounts error:', err);
        alert(
          'API error. Status: ' + err.status + ' | ' +
          (err.error?.message || err.message ||
            'Unable to load loan accounts.')
        );
      }
    });
  }

  info: any = null;
  outstandingPrincipal: any = null;
  NextEmiDate: any = null;
  loanAccountNo: any = null;

  getCardsData(loanAccountId: number): void {
    this.service.getCardsData(loanAccountId).subscribe({
      next: (res: any) => {
        console.log('Loan summary:', res);

        const data = res?.data ?? res?.Data ?? res;

        this.info = data;
        this.outstandingPrincipal = data?.outstandingPrincipal;
        this.NextEmiDate = data?.nextEmiDate;
        this.loanAccountNo = data?.loanAccountNo;

        this.cd.detectChanges();
      },
      error: err => {
        console.error('Loan summary error:', err);
        alert(
          'API error. Status: ' + err.status + ' | ' +
          (err.error?.message || err.message ||
            'Unable to load loan summary.')
        );
      }
    });
  }

  onLoanChange(loanAccountId: number | string | null): void {
    if (loanAccountId) {
      this.getCardsData(Number(loanAccountId));
    } else {
      this.info = null;
      this.outstandingPrincipal = null;
      this.NextEmiDate = null;
      this.loanAccountNo = null;
    }
  }

  CustomerId: any;
  ForeClosureType: any = '';
  PartialAmount: any = null;
  ExpectedClosureDate: any = '';
  Reason: any = '';

  SaveForeClosure(): void {
    if (!this.SelectedLoanAccount) {
      alert('Please select a loan account.');
      return;
    }

    if (!this.ForeClosureType) {
      alert('Please select a foreclosure type.');
      return;
    }

    if (
      this.ForeClosureType === 'Partial' &&
      (!this.PartialAmount || Number(this.PartialAmount) <= 0)
    ) {
      alert('Please enter a valid partial amount.');
      return;
    }

    if (!this.ExpectedClosureDate) {
      alert('Please select an expected closure date.');
      return;
    }

    const data = {
      customerId: this.CustomerId,
      loanAccountId: Number(this.SelectedLoanAccount),
      foreClosureType: this.ForeClosureType,
      partialAmount:
        this.ForeClosureType === 'Partial'
          ? Number(this.PartialAmount)
          : 0,
      expectedClosureDate: new Date(
        this.ExpectedClosureDate
      ).toISOString(),
      reason: this.Reason || 'NA'
    };

    console.log('Submitting foreclosure request:', data);

    this.service.AddForeClosure(data).subscribe({
      next: (res: any) => {
        console.log('Foreclosure response:', res);

        alert(
          res?.message ||
          'Foreclosure request submitted successfully.'
        );

        this.SelectedLoanAccount = null;
        this.info = null;
        this.outstandingPrincipal = null;
        this.NextEmiDate = null;
        this.loanAccountNo = null;

        this.ForeClosureType = '';
        this.PartialAmount = null;
        this.ExpectedClosureDate = '';
        this.Reason = '';

        this.getForeClosureRequests();
      },
      error: err => {
        console.error('Submit foreclosure error:', err);
        alert(
          err.error?.message ||
          'Failed to submit foreclosure request. Status: ' + err.status
        );
      }
    });
  }

  private getRequestId(request: any): number | null {
    const rawId =
      request?.requestId ??
      request?.RequestId ??
      request?.foreClosureRequestId ??
      request?.ForeClosureRequestId ??
      request?.foreclosureRequestId ??
      request?.ForeclosureRequestId;

    if (
      rawId === undefined ||
      rawId === null ||
      String(rawId).trim() === '' ||
      !Number.isInteger(Number(rawId)) ||
      Number(rawId) <= 0
    ) {
      return null;
    }

    return Number(rawId);
  }

  payForeClosure(request: any): void {
    console.log('Selected foreclosure request:', request);
    console.log('Selected request fields:', Object.keys(request ?? {}));

    const requestId = this.getRequestId(request);
    const status = request?.status ?? request?.Status;

    if (requestId === null) {
      console.error(
        'Request ID missing. Full request:',
        JSON.stringify(request, null, 2)
      );

      alert(
        'Request ID is missing from the API response. ' +
        'Check the browser console and confirm that the backend returns RequestId.'
      );
      return;
    }

    if (String(status).toLowerCase() !== 'approved') {
      alert('Only approved foreclosure requests can be paid.');
      return;
    }

    if (this.payingRequestId !== null) {
      return;
    }

    this.payingRequestId = requestId;

    this.service.createPaymentOrder(requestId).subscribe({
      next: (res: any) => {
        console.log('Payment order response:', res);

        const order = res?.data ?? res?.Data ?? res;

        if (
          !order?.keyId ||
          !order?.orderId ||
          order?.amountInPaise == null
        ) {
          this.payingRequestId = null;

          console.error('Invalid payment order response:', order);
          alert('Payment order response is missing required details.');
          return;
        }

        const options = {
          key: order.keyId,
          amount: order.amountInPaise,
          currency: order.currency || 'INR',
          order_id: order.orderId,
          name: 'LoanApp',
          description: 'Foreclosure Payment',

          handler: (response: any) => {
            const paymentData = {
              requestId: requestId,
              razorpayOrderId: response.razorpay_order_id,
              razorpayPaymentId: response.razorpay_payment_id,
              razorpaySignature: response.razorpay_signature
            };

            this.service.verifyPayment(paymentData).subscribe({
              next: (verifyRes: any) => {
                this.payingRequestId = null;

                alert(
                  verifyRes?.message ||
                  'Payment verified successfully.'
                );

                this.getForeClosureRequests();
              },
              error: err => {
                this.payingRequestId = null;

                console.error('Payment verification error:', err);

                alert(
                  'Verification failed. Status: ' + err.status + ' | ' +
                  (err.error?.message ||
                    JSON.stringify(err.error) ||
                    'Payment verification failed.')
                );
              }
            });
          },

          modal: {
            ondismiss: () => {
              this.payingRequestId = null;
            }
          }
        };

        try {
          if (typeof Razorpay === 'undefined') {
            throw new Error(
              'Razorpay is not defined. Check checkout.js in src/index.html.'
            );
          }

          const checkout = new Razorpay(options);

          checkout.on('payment.failed', (response: any) => {
            this.payingRequestId = null;

            console.error('Razorpay payment failed:', response);

            alert(
              response.error?.description ||
              'Payment failed.'
            );
          });

          checkout.open();
        } catch (error) {
          this.payingRequestId = null;

          console.error('Checkout error:', error);
          alert('Checkout error: ' + String(error));
        }
      },
      error: err => {
        this.payingRequestId = null;

        console.error('Create payment order error:', err);

        alert(
          'Status: ' + err.status + ' | ' +
          (err.error?.message ||
            JSON.stringify(err.error) ||
            'Unable to create payment order.')
        );
      }
    });
  }
}
