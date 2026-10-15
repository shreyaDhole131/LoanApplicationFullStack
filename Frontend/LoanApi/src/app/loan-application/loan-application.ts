import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Applyloan } from '../services/applyloan';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-loan-application',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './loan-application.html',
  styleUrl: './loan-application.css',
})
export class LoanApplication {
  constructor(private service: Applyloan) {

  }

  SaveLoan(data: any) {
    const payload = {
      loantype: data.loantype,
      loanAmount: Number(data.loanAmount),
      tenureMonths: Number(data.tenureMonths),
      bankName: data.bankName,
      bankAccountNumber: data.bankAccountNumber,
      ifscCode: data.ifscCode,
      emiDay: Number(data.emiDay)
    }
    this.service.SaveLoan(payload).subscribe({
      next: res => {
        alert(res.message)
      },
      error: err => {
        console.log(err);
      }
    })
  }

  loans = new FormGroup({
    loantype: new FormControl(''),
    loanAmount: new FormControl(''),
    tenureMonths: new FormControl(''),
    interestRate: new FormControl(''),
    emiAmount: new FormControl(''),
    bankName: new FormControl(''),
    bankAccountNumber: new FormControl(''),
    ifscCode: new FormControl(''),
    emiDay: new FormControl('')
  });

  getPreview() {
    const amount = Number(this.loans.value.loanAmount);
    const tenure = Number(this.loans.value.tenureMonths);

    if (!amount || !tenure) return;

    this.service.Preview({ loanAmount: amount, tenureMonths: tenure }).subscribe({
      next: (res) => {
        this.loans.patchValue({
          interestRate: res.interestRate,
          emiAmount: res.emiAmount
        });
      },
      error: (err) => console.log(err)

    });


  }
}
