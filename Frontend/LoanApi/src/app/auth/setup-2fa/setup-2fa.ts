import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef 
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../auth';

@Component({
  selector: 'app-setup-2fa',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './setup-2fa.html',
  styleUrl: './setup-2fa.css'
})
export class Setup2fa implements OnInit {

  private auth = inject(Auth);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  loading = false;
  otp = '';
  qrImageUrl = '';
  errorMessage = '';
  successMessage = '';

  // =========================
  // PAGE LOAD
  // =========================

  ngOnInit(): void {
    console.log('Setup 2FA page loaded.');

    const challengeToken = this.auth.getChallengeToken();
    console.log('Challenge token exists:', !!challengeToken);

    if (!challengeToken) {
      this.errorMessage = 'Authentication session expired. Please login again.';
      // Force UI update in case the token is missing on load
      this.cdr.detectChanges();
      return;
    }

    this.loadQrCode();
  }

  // =========================
  // LOAD QR CODE
  // =========================

  loadQrCode(): void {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.auth.setup2FA().subscribe({
      next: (response: any) => {
        console.log('Setup 2FA response:', response);

        if (response && response.qrCodeDataUrl) {
          this.qrImageUrl = response.qrCodeDataUrl;
          console.log('QR code received successfully.');
        } else {
          console.error('QR code missing:', response);
          this.errorMessage = 'QR code was not returned by the server.';
        }

        this.loading = false;
        this.cdr.detectChanges(); // <-- Forces the QR code to render
      },
      error: (error: any) => {
        console.error('Setup 2FA error:', error);
        this.loading = false;

        // Shows the exact backend error message
        this.errorMessage = error.error?.message || 'Failed to load QR code.';
        this.cdr.detectChanges(); 
      }
    });
  }

  // =========================
  // VERIFY 2FA
  // =========================

  verifySetup(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!/^\d{6}$/.test(this.otp)) {
      this.errorMessage = 'Please enter a valid 6-digit code.';
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;

    this.auth.verify2FASetup({ otp: this.otp }).subscribe({
      next: (response: any) => {
        console.log('2FA verification response:', response);

        this.loading = false;
        this.successMessage = 'Two-factor authentication enabled successfully!';
        this.auth.clearChallengeToken();

        if (response?.token) {
          localStorage.setItem('token', response.token);
          console.log('Final JWT token saved.');
        }

        this.cdr.detectChanges(); // <-- Forces the success message to show

        // Give the user 1.5 seconds to read the success message, then go to dashboard
        setTimeout(() => {
          if (response.roleId == 1) {
            this.router.navigate(['/customer-dashboard']);
          } else if (response.roleId == 2) {
            this.router.navigate(['/officer-dashboard']);
          } else {
            this.router.navigate(['/login']);
          }
        }, 1500);
      },
      error: (error: any) => {
        console.error('2FA verification error:', error);
        this.loading = false;
        
        // Shows exactly what the backend says (e.g. "Invalid authenticator OTP")
        this.errorMessage = error.error?.message || 'Invalid code. Please try again.';
        this.cdr.detectChanges(); // <-- Forces the error message to show
      }
    });
  }
}