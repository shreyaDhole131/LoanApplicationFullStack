
import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../auth';

@Component({
  selector: 'app-setup-two-fa',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './setup-2fa.html',
  styleUrl: './setup-2fa.css'
})
export class Setup2FA implements OnInit, OnDestroy {
  private auth = inject(Auth);
  private router = inject(Router);

  qrCodeUrl: string | null = null;
  otp = '';
  loading = true;
  verifying = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadQRCode();
  }

  loadQRCode(): void {
    this.loading = true;
    this.errorMessage = '';

    this.auth.get2FAQr().subscribe({
      next: (response: Blob) => {
        console.log('QR response:', response);
        console.log('QR size:', response.size);
        console.log('QR type:', response.type);

        this.loading = false;

        if (!response || response.size === 0) {
          this.errorMessage = 'The QR code image is empty.';
          return;
        }

        if (response.type && !response.type.startsWith('image/')) {
          this.errorMessage =
            'The API returned data that is not an image. Check the backend QR endpoint.';
          console.error('Unexpected QR content type:', response.type);
          return;
        }

        if (this.qrCodeUrl) {
          URL.revokeObjectURL(this.qrCodeUrl);
        }

        this.qrCodeUrl = URL.createObjectURL(response);
      },

      error: (err: any) => {
        this.loading = false;

        console.error('QR request failed:', err);
        console.error('HTTP status:', err.status);
        console.error('API response:', err.error);

        this.errorMessage =
          err.status === 401 || err.status === 403
            ? 'Authorization failed. Please log in again.'
            : 'Unable to load the QR code. Check the browser console.';
      }
    });
  }

  verifyOTP(): void {
    if (!/^\d{6}$/.test(this.otp)) {
      this.errorMessage = 'Please enter a valid 6-digit code.';
      return;
    }

    this.verifying = true;
    this.errorMessage = '';

    this.auth.verify2FASetup({ otp: this.otp }).subscribe({
      next: (res: any) => {
        this.verifying = false;

        if (!res.token) {
          this.errorMessage =
            res.message || 'Authentication token was not returned.';
          return;
        }

        localStorage.setItem('token', res.token);
        sessionStorage.removeItem('challengeToken');

        if (this.qrCodeUrl) {
          URL.revokeObjectURL(this.qrCodeUrl);
          this.qrCodeUrl = null;
        }

        if (res.roleId == 1) {
          this.router.navigate(['/customer-dashboard']);
        } else if (res.roleId == 2) {
          this.router.navigate(['/officer-dashboard']);
        } else {
          this.errorMessage = 'Unknown user role.';
        }
      },

      error: (err: any) => {
        this.verifying = false;

        console.error('OTP verification failed:', err);

        this.errorMessage =
          err.error?.message || 'Authenticator verification failed.';
      }
    });
  }

  ngOnDestroy(): void {
    if (this.qrCodeUrl) {
      URL.revokeObjectURL(this.qrCodeUrl);
    }
  }
}
