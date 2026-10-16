import { Component } from '@angular/core';
import { Officer } from '../officer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-customers',
  imports: [Sidebar],
  templateUrl: './customers.html',
  styleUrl: './customers.css'
})
export class Customers {
  customers: any[] = [];
  selectedCustomer: any = null;

  constructor(private service: Officer) {
    this.getCustomers();
  }

  getCustomers() {
    this.service.getAllCustomers().subscribe({
      next: (res: any[]) => {
        this.customers = res;
      },
      error: (err: any) => {
        console.log(err);
      }
    });
  }

  viewProfile(customerId: number) {
    this.service.getCustomerProfile(customerId).subscribe({
      next: (res: any) => {
        console.log('CUSTOMER PROFILE', res);
        this.selectedCustomer = res;
      },
      error: (err: any) => {
        console.log(err);
      }
    });
  }

  backToCustomers() {
    this.selectedCustomer = null;
  }
}