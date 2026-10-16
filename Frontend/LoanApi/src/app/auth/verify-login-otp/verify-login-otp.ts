
import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../auth';

@Component({
  selector: 'app-verify-login-otp',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './verify-login-otp.html',
  styleUrl: './verify-login-otp.css'
})
export class VerifyLoginOtp {
  private auth = inject(Auth);
  private router = inject(Router);

  otp = '';
  loading = false;
  errorMessage = '';

  verifyOTP() {
    if (!/^\d{6}$/.test(this.otp)) {
      this.errorMessage = 'Please enter a valid 6-digit code.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    this.auth.verifyLoginOtp({ otp: this.otp }).subscribe({
      next: (res: any) => {
        this.loading = false;

        if (!res.token) {
          this.errorMessage = 'Authentication token was not returned.';
          return;
        }

        localStorage.setItem('token', res.token);
        sessionStorage.removeItem('challengeToken');

        if (res.roleId == 1) {
          this.router.navigate(['/customer-dashboard']);
        } else if (res.roleId == 2) {
          this.router.navigate(['/officer-dashboard']);
        } else {
          this.errorMessage = 'Unknown user role.';
        }
      },
      error: (err: any) => {
        console.error(err);
        this.loading = false;
        this.errorMessage =
          err.error?.message || 'OTP verification failed.';
      }
    });
  }
}
