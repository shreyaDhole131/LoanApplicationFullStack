import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, PLATFORM_ID, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Customerservice } from '../services/customerservice';
import { Sidebar } from '../customer/sidebar/sidebar';


@Component({
  selector: 'app-customerloanaccounts',
  standalone: true,
  imports: [CommonModule, FormsModule,Sidebar],
  templateUrl: './costomerloanacc.html',
  styleUrl: './costomerloanacc.css'
})
export class Customerloanaccounts {

  selectedStatus: string = '';
  searchText: string = '';

  allLoans: any[] = [];
  filteredLoans: any[] = [];

  private platformId = inject(PLATFORM_ID);

  constructor(
    private service: Customerservice,
    private router: Router
  ) {
    this.getMyLoans();
  }

  getMyLoans(): void {

    this.service.getMyLoanAccounts().subscribe({

      next: (data) => {

        console.log('My loan accounts:', data);

        this.allLoans = data;
        this.filteredLoans = data;

      },

      error: (err) => {

        console.error(
          'Error loading customer loan accounts:',
          err
        );

        this.allLoans = [];
        this.filteredLoans = [];

      }

    });
  }

  filterLoans(): void {

    this.filteredLoans = this.allLoans.filter((loan: any) => {

      return this.selectedStatus === '' ||
        loan.loanStatus?.toLowerCase() ===
        this.selectedStatus.toLowerCase();

    });

  }

  searchLoans(): void {

    const search = this.searchText.trim().toLowerCase();

    if (search === '') {

      this.filterLoans();

      return;
    }

    this.filteredLoans = this.allLoans.filter((loan: any) => {

      return (
        loan.loanAccountNo?.toLowerCase().includes(search) ||
        loan.customerName?.toLowerCase().includes(search) ||
        loan.loanStatus?.toLowerCase().includes(search)
      );

    });

  }

  viewLoan(loanAccountId: number): void {

    this.router.navigate([
      '/loanaccount/details',
    loanAccountId
    ]);

  }
}