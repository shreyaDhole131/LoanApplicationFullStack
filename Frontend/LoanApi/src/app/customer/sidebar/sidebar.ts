import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {

  customerOpen = true;
  loanOpen = true;
  paymentOpen = true;

  toggleCustomer() {
    this.customerOpen = !this.customerOpen;
  }

  toggleLoan() {
    this.loanOpen = !this.loanOpen;
  }

  togglePayment() {
    this.paymentOpen = !this.paymentOpen;
  }
}