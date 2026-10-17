import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Auth } from '../auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {

  auth = inject(Auth);
  router = inject(Router);

  model = {
    email: '',
    password: ''
  };

  // =========================
  // PAGE LOAD (ADDED THIS)
  // =========================
  ngOnInit(): void {
    // Wipe any old tokens when the login page loads to prevent stale tokens
    this.auth.clearChallengeToken();
    localStorage.removeItem('token');
    console.log('Login page loaded: Cleared old tokens.');
  }

  // =========================
  // LOGIN
  // =========================

  login(): void {
    // 1. Clear any old token before trying to log in
    this.auth.clearChallengeToken();

    this.auth.login(this.model).subscribe({
      next: (res: any) => {
        console.log('Login response:', res);

        // =================================
        // 2FA CHALLENGE
        // =================================
        if (res.challengeToken) {
          // Save the new token
          this.auth.saveChallengeToken(res.challengeToken);
          console.log('Challenge token stored successfully.');

          // Route to Setup 2FA
          if (res.nextStep === 'setup-2fa' || res.nextStep === 'setup2FA') {
            this.router.navigate(['/setup-2fa']);
            return;
          }

          // Route to Verify Login OTP
          if (res.nextStep === 'verify-login-otp' || res.nextStep === 'verifyLoginOtp') {
            this.router.navigate(['/verify-login-otp']);
            return;
          }

          // Unknown step
          this.auth.clearChallengeToken();
          alert('Unknown authentication step returned by the server.');
          return;
        }

        // =================================
        // NORMAL LOGIN TOKEN (No 2FA)
        // =================================
        if (res.token) {
          localStorage.setItem('token', res.token);
          this.auth.clearChallengeToken();
          console.log('Full login token saved.');

          if (res.roleId == 1) {
            this.router.navigate(['/customer-dashboard']);
            return;
          }
          if (res.roleId == 2) {
            this.router.navigate(['/officer-dashboard']);
            return;
          }

          alert('Unknown user role.');
          return;
        }

        alert('Login response does not contain a token.');
      },
      
      error: (err: any) => {
        console.error('Login HTTP status:', err.status);
        console.error('Login response:', err.error);
        alert(err.error?.message || err.error?.title || 'Login failed. Check your email and password.');
      }
    });
  }

  // =========================
  // REGISTER
  // =========================

  register(): void {
    this.router.navigate(['/register']);
  }
}