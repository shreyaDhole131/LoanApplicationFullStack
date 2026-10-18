import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Loanservice } from '../services/loanservice';
import { ActivatedRoute, Router } from '@angular/router';
import { Sidebar } from '../customer/sidebar/sidebar';

@Component({
  imports: [CommonModule,Sidebar],
  selector: 'app-loandetails',
  styleUrl: './loandetails.css',
  templateUrl: './loandetails.html',
})
export class Loandetails {

loan: any = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: Loanservice
  ) {
    this.getLoanDetails();
  }

  getLoanDetails(): void {

    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getLoanAccountById(id).subscribe({
      next: res => {
        console.log('Loan Details:', res);
        this.loan = res;
      },
      error: err => {
        console.log(err);
      }
    });

  }

  goBack(): void {
    this.router.navigate(['/loanaccount']);
  }
}
