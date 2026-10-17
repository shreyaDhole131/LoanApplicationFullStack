import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-officer-sidebar',
  imports: [RouterLink],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {

  customerOpen = false;
  loanOpen = false;
  requestOpen = false;

  toggleCustomer() {
    this.customerOpen = !this.customerOpen;
  }

  toggleLoan() {
    this.loanOpen = !this.loanOpen;
  }

  toggleRequest() {
    this.requestOpen = !this.requestOpen;
  }

}