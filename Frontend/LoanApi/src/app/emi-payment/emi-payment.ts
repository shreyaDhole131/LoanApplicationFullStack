
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { EmiPaymentService } from '../services/emi-payment.service';

@Component({
  selector: 'app-emi-payment',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './emi-payment.html',
  styleUrl: './emi-payment.css',
})
export class EmiPayment implements OnInit {

  service = inject(EmiPaymentService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  emiScheduleId: number = 0;
  info: any;
  errorMessage: string = '';

  ngOnInit() {
    this.emiScheduleId = Number(
      this.route.snapshot.paramMap.get('emiScheduleId')
    );

    if (this.emiScheduleId > 0) {
      this.getPaymentDetails();
    } else {
      this.errorMessage = 'Invalid EMI schedule ID';
    }
  }

  getPaymentDetails() {
    this.service.getPaymentDetails(this.emiScheduleId).subscribe({
      next: (res) => {
        this.info = res;
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || 'Unable to load EMI payment details';
      }
    });
  }

  viewBreakup() {
    this.router.navigate(['/emi-payment', this.emiScheduleId, 'breakup']);
  }

  
payNow() {
  this.service.createPaymentOrder(this.emiScheduleId).subscribe({
    next: (res) => {
      console.log('Razorpay order response:', res);
    },
    error: (err) => {
      console.error(err);
      this.errorMessage =
        err.error?.message || 'Unable to create payment order';
    }
  });
}


  goBack() {
    this.router.navigate(['/emi-schedule']);
  }
}
