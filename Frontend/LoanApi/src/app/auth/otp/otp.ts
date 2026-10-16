
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Auth } from '../auth';

@Component({
  selector: 'app-otp',
  standalone: true,
  imports: [FormsModule],
  styleUrl: './otp.css',
  templateUrl: './otp.html'
})
export class Otp {

  auth = inject(Auth);
  route = inject(ActivatedRoute);
  router = inject(Router);

  model: any = {
    email: '',
    otp: ''
  };

  constructor() {
    this.route.queryParams.subscribe(params => {
      this.model.email = params['email'] || '';
    });
  }

  verifyOtp() {
    this.auth.otp(this.model).subscribe({
      next: res => {
        alert(res.message);

        // Navigate to login after successful verification
        this.router.navigate(['/login']);
      },
      error: err => {
        console.log(err);
        alert(err.error?.message || 'OTP verification failed');
      }
    });
  }
}
