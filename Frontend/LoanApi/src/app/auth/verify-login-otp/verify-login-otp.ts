import { CommonModule } from '@angular/common';
import { Component, inject, ChangeDetectorRef } from '@angular/core';
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
  private cdr = inject(ChangeDetectorRef);

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

    try {
      this.auth.verifyLoginOtp({ otp: this.otp }).subscribe({
        next: (res: any) => {
          this.loading = false;

          if (!res.token) {
            this.errorMessage = 'Authentication token was not returned.';
            this.cdr.detectChanges();
            return;
          }

          // Save final JWT and clear the challenge token
          localStorage.setItem('token', res.token);
          sessionStorage.removeItem('challengeToken');

          // Route based on user role
          if (res.roleId == 1) {
            this.router.navigate(['/customer-dashboard']);
          } else if (res.roleId == 2) {
            this.router.navigate(['/officer-dashboard']);
          } else {
            this.errorMessage = 'Unknown user role.';
            this.cdr.detectChanges();
          }
        },
        error: (err: any) => {
          console.error('FULL ERROR OBJECT:', err);
          this.loading = false;
          
          // Handles HTTP errors (from backend) and custom errors from auth.ts
          this.errorMessage = err.error?.message || err.message || 'OTP verification failed. Please try again.';
          
          this.cdr.detectChanges();
        }
      });
    } catch (error: any) {
      // This catches the synchronous errors thrown by auth.ts 
      // (e.g., "Challenge token missing" or "Invalid token stage")
      this.loading = false;
      this.errorMessage = error.message || 'An unexpected error occurred.';
      this.cdr.detectChanges();
    }
  }
}