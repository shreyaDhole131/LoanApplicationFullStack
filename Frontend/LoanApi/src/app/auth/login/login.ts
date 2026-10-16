
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  styleUrl: './login.css',
  templateUrl: './login.html'
})
export class Login {

  auth = inject(Auth);
  router = inject(Router);

  model: any = {
    email: '',
    password: ''
  };

  login() {
    this.auth.login(this.model).subscribe({
      next: res => {
        console.log(res);

        // First-time login: configure 2FA
        if (res.nextStep === 'setup-2fa') {

          sessionStorage.setItem(
            'challengeToken',
            res.challengeToken
          );

          this.router.navigate(['/setup-2fa']);
        }

        // Subsequent login: verify authenticator OTP
        else if (res.nextStep === 'verify-login-otp') {

          sessionStorage.setItem(
            'challengeToken',
            res.challengeToken
          );

          this.router.navigate(['/verify-login-otp']);
        }

        // If API directly returns a final token
        else if (res.token) {

          localStorage.setItem('token', res.token);

          alert('Login successful');

          if (res.roleId == 1) {
            this.router.navigate(['/customer-dashboard']);
          }
          else if (res.roleId == 2) {
            this.router.navigate(['/officer-dashboard']);
          }
          else {
            alert('Unknown user role');
          }
        }

        else {
          alert(res.message || 'Unexpected login response');
        }
      },

      error: err => {
        console.log(err);
        alert(err.error?.message || 'Login failed');
      }
    });
  }

  register() {
    this.router.navigate(['/register']);
  }
}
