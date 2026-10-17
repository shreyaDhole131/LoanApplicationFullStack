
import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { customer_loans } from '../services/customer-loans';

@Component({
  selector: 'app-payment-history',
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-history.html',
  styleUrl: './payment-history.css'
})
export class PaymentHistory {

  constructor(private service:customer_loans, private cd:ChangeDetectorRef){
    this.getLoanAccounts();
    this.getPaymentHistory()
  }


LoanAccounts:any;
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


 paymentHistory: any;
SelectedLoanAccount: number | null = null;
SelectedMonths = 0;

getPaymentHistory() {
  this.service.getPaymentHistory(this.SelectedLoanAccount, this.SelectedMonths).subscribe(
    res => {
    console.log(res);
    this.paymentHistory = res.data;
    this.cd.detectChanges();
  });
}

applyFilters() {
  this.getPaymentHistory();
}


searchText = '';

get filteredPayments(): any[] {
  return this.paymentHistory.filter((payment: any) =>
    Object.values(payment).some(value =>
      String(value ?? '').toLowerCase()
        .includes(this.searchText.toLowerCase())
    )
  );
}

}
