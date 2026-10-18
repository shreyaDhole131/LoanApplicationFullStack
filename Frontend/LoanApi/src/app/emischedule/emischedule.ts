import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Emischeduleservice } from '../services/emischeduleservice';
import { NotificationComponent } from '../notification/notification';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../customer/sidebar/sidebar';
import { Router } from '@angular/router';   // ADD THIS

@Component({
  imports: [CommonModule, NotificationComponent, FormsModule, Sidebar],
  selector: 'app-emischedule',
  styleUrl: './emischedule.css',
  templateUrl: './emischedule.html',
})
export class Emischedule implements OnInit {

  loanAccounts: any[] = [];

  selectedLoanAccountId: number | null = null;

  allEmi: any[] = [];

  paidEmiCount = 0;
  paidEmiAmount = 0;

  pendingEmiCount = 0;
  pendingEmiAmount = 0;

  remainingEmiCount = 0;
  remainingBalance = 0;


  constructor(
    private service: Emischeduleservice,
    private router: Router              // ADD THIS
  ) 
  {
  }


  ngOnInit(): void {
    this.loadLoanAccounts();
  }



  loadLoanAccounts() {

    console.log('Loading loan accounts...');

    this.service.getMyLoanAccounts().subscribe({

      next: (res) => {

        console.log('Loan Accounts:', res);

        this.loanAccounts = res;


        if (this.loanAccounts.length > 0) {

          const loanAccountId =
            this.loanAccounts[0].loanAccountId;

          this.selectedLoanAccountId =
            loanAccountId;

          console.log(
            'Automatically selected Loan Account:',
            loanAccountId
          );

          this.fetchEmi(loanAccountId);
        }

      },

      error: (err) => {

        console.error(
          'Error loading loan accounts:',
          err
        );

      }

    });

  }




  onLoanAccountChange(loanAccountId: number) {

    console.log(
      'Selected Loan Account:',
      loanAccountId
    );

    this.selectedLoanAccountId =
      loanAccountId;

    if (loanAccountId) {

      this.fetchEmi(loanAccountId);

    }

  }



  fetchEmi(loanAccountId: number) {

    this.service
      .fetchEmiSchedule(loanAccountId)
      .subscribe({

        next: (res) => {

          console.log(
            'EMI Schedule Response:',
            res
          );

          this.allEmi =
            res.schedules ?? [];


          this.paidEmiCount =
            res.paidEmiCount ?? 0;

          this.paidEmiAmount =
            res.paidEmiAmount ?? 0;

          this.pendingEmiCount =
            res.pendingEmiCount ?? 0;

          this.pendingEmiAmount =
            res.pendingEmiAmount ?? 0;

          this.remainingEmiCount =
            res.remainingEmiCount ?? 0;

          this.remainingBalance =
            res.remainingBalance ?? 0;

        },

        error: (err) => {

          console.error(
            'Error loading EMI schedule:',
            err
          );

          this.allEmi = [];

        }

      });

  }


  canPay(index: number): boolean {

    const currentEmi =
      this.allEmi[index];

    if (currentEmi.paymentStatus === 'Paid') {
      return false;
    }

    const today = new Date();

    today.setHours(
      0,
      0,
      0,
      0
    );

    const dueDate =
      new Date(currentEmi.dueDate);

    dueDate.setHours(
      0,
      0,
      0,
      0
    );


    if (
      dueDate.getTime() !==
      today.getTime()
    ) {

      return false;

    }


    if (index === 0) {

      return currentEmi.paymentStatus === 'Pending';

    }


    const previousEmi =
      this.allEmi[index - 1];


    return (
      previousEmi.paymentStatus === 'Paid' &&
      currentEmi.paymentStatus === 'Pending'
    );

  }


  payEmi(emi: any) {

    const index =
      this.allEmi.indexOf(emi);

    if (!this.canPay(index)) {
      return;
    }

    console.log(
      "Payment allowed for installment:",
      emi.installmentNo
    );

    this.router.navigate([
      '/emi-payment',
      emi.emiScheduleId
    ]);
  }

}