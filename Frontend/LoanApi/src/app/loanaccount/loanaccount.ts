import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { Loanservice } from '../services/loanservice';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../customer/sidebar/sidebar';

@Component({
  selector: 'app-loanaccount',
  imports: [CommonModule,FormsModule,Sidebar],
  templateUrl: './loanaccount.html',
  styleUrl: './loanaccount.css',
})
export class Loanaccount {
   constructor(private service:Loanservice,private router: Router)
  {
    this.getAllLoans();

  }

  selectedStatus: string = '';
searchText: string = '';

filteredLoans: any[] = [];

allLoans: any[] = [];
  getAllLoans():void
  {
    this.service.getLoanAccounts().subscribe({
  next: (data) => {
    this.allLoans = data;
    this.filteredLoans = data;
  },
  error: (err) => {
    console.error('Error loading loan accounts:', err);
  }
});
  }

  selectedLoan: any = null;

viewLoan(loanAccountId: number): void {

  this.router.navigate([
    '/loanaccount/details',
    loanAccountId
  ]);

}

filterLoans(): void {

  this.filteredLoans = this.allLoans.filter(loan => {

    return this.selectedStatus === '' ||
      loan.loanStatus?.toLowerCase() ===
      this.selectedStatus.toLowerCase();

  });
}


searchLoans(): void {

  const search = this.searchText.trim();

  if (search === '') {

    this.filterLoans();

    return;
  }

  this.service.searchLoanAccounts(search).subscribe({

    next: (data) => {

      this.filteredLoans = data;

    },

    error: (err) => {

      console.error(
        'Error searching loan accounts:',
        err
      );

      this.filteredLoans = [];

    }

  });

}

}
