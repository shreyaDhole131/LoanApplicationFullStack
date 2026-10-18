
import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { customer_loans } from '../services/customer-loans';
import { nextTick } from 'process';
import { Sidebar } from '../customer/sidebar/sidebar';

declare const Razorpay: any;
declare var $:any;
@Component({
  selector: 'app-foreclosure-request',
    standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule,Sidebar],
  templateUrl: './foreclosure-request.html',
  styleUrl: './foreclosure-request.css'
})
export class foreclosure_request{
  constructor(private service:customer_loans, private cd:ChangeDetectorRef){
    this.getLoanAccounts();
    this.getForeClosureRequests();

    //this.getPaymentHistory()
  }

  allrequests:any;
  payingRequestId: number | null = null;
  getForeClosureRequests() {
    this.service.getForeClosureRequests().subscribe({
      next:res=>{
        console.log(res);
        this.allrequests=res.data;
        this.cd.detectChanges();
      },
      error:err=>{
        console.log(err);
      }

    });
  }

  LoanAccounts:any;
  SelectedLoanAccount: any;
  getLoanAccounts(){
    this.service.getLoanAccounts().subscribe({
      next:res=>{
        console.log(res);
        this.LoanAccounts=res.data;
        this.cd.detectChanges();
      },
      error:err=>{
        console.log(err);
      }
    });
  }
    info:any;
    outstandingPrincipal:any;
    NextEmiDate:any;
    loanAccountNo:any;
    getCardsData(loanAccountId:number){
      this.service.getCardsData(loanAccountId).subscribe({
        next:res=>{
          console.log(res);
          this.info=res.data;
          this.outstandingPrincipal=res.data.outstandingPrincipal;
          this.NextEmiDate=res.data.nextEmiDate;
          this.loanAccountNo=res.data.loanAccountNo;
          this.cd.detectChanges();
        },
        error:err=>{
          console.log(err);
        }
      });
    }

onLoanChange(loanAccountId: number | null): void {
  if (loanAccountId) {
    this.getCardsData(Number(loanAccountId));
  } else {
    this.info = null;
    this.outstandingPrincipal = null;
    this.NextEmiDate = null;
    this.loanAccountNo = null;
  }
}


   
      CustomerId:any;
      ForeClosureType:any;
      PartialAmount:any;
      ExpectedClosureDate:any;
      Reason:any;
    SaveForeClosure(){
      if(!this.SelectedLoanAccount){
        alert('Please Select a loan account');
        return;
      }
     
      if (!this.ForeClosureType) {
  alert('Please select a foreclosure type');
  return;
}

if (this.ForeClosureType === 'Partial' &&(!this.PartialAmount || this.PartialAmount <= 0))
   {
  alert('Please enter a valid partial amount');
  return;
}

if (!this.ExpectedClosureDate) {
  alert('Please select an expected closure date');
  return;
}
      const data = {
  customerId: this.CustomerId,
  loanAccountId: Number(this.SelectedLoanAccount),
  foreClosureType: this.ForeClosureType,
  partialAmount: this.ForeClosureType === 'Partial' ? this.PartialAmount : null,
  expectedClosureDate: this.ExpectedClosureDate ? new Date(this.ExpectedClosureDate).toISOString() : null,
  reason: this.Reason || 'NA'
};
      console.log(data);

      this.service.AddForeClosure(data).subscribe({
        next: res => {
  alert(res.message || 'Foreclosure request submitted successfully.');
  console.log(res);

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
        error:err=>{
          console.log(err);
          alert(err.error?.message || 'failed to submit Foreclosure request')
        }
      });
  }

  payForeClosure(request: any): void {
  if (request.status !== 'Approved' || this.payingRequestId !== null) {
    return;
  }

  this.payingRequestId = request.requestId;

  this.service.createPaymentOrder(request.requestId).subscribe({
    next: res => {
      const order = res?.data ?? res;

      const options = {
        key: order.keyId,
        amount: order.amountInPaise,
        currency: order.currency,
        order_id: order.orderId,
        name: 'LoanApp',
        description: 'Foreclosure Payment',

        handler: (response: any) => {
          const data = {
            requestId: order.requestId ?? request.requestId,
            razorpayOrderId: response.razorpay_order_id,
            razorpayPaymentId: response.razorpay_payment_id,
            razorpaySignature: response.razorpay_signature
          };

          this.service.verifyPayment(data).subscribe({
            next: verifyRes => {
              this.payingRequestId = null;
              alert(verifyRes?.message || 'Payment verified successfully.');
              this.getForeClosureRequests();
            },
            error: err => {
              this.payingRequestId = null;
              alert(err.error?.message || 'Payment verification failed.');
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
        const checkout = new Razorpay(options);

        checkout.on('payment.failed', (response: any) => {
          this.payingRequestId = null;
          alert(response.error?.description || 'Payment failed.');
        });

        checkout.open();
      } catch {
        this.payingRequestId = null;
        alert('Unable to open Razorpay Checkout.');
      }
    },
    error: err => {
      this.payingRequestId = null;
      alert(err.error?.message || 'Unable to create payment order.');
    }
  });
  
}



}
