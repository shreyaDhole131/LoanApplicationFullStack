
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../auth';
@Component({
  selector: 'app-register',
  standalone: true,
imports: [FormsModule, RouterLink],
  styleUrl: './register.css',
  templateUrl: './register.html'
})
export class Register {
  auth = inject(Auth);
  router = inject(Router);

  model: any = {
    firstName: '',
    lastName: '',
    age: 0,
    email: '',
    password: '',
    mobileNo: '',
    pan: '',
    aadhaarNo: '',
    employmentType: '',
    monthlyIncome: ''
  };

  register() {
    console.log(
      'Submitted registration data:',
      JSON.stringify(this.model, null, 2)
    );

    this.auth.register(this.model).subscribe({
      next: (res: any) => {
        console.log('Registration response:', res);

        alert(res.message || 'Registration successful');

        this.router.navigate(['/otp'], {
          queryParams: { email: this.model.email }
        });
      },

      error: (err: any) => {
        console.log('HTTP Status:', err.status);
        console.log(
          'Validation errors:',
          JSON.stringify(err.error?.errors, null, 2)
        );
        console.log('Full API response:', err.error);

        const errors = err.error?.errors;

        if (errors) {
          const messages = Object.entries(errors)
            .map(([field, details]) => {
              const text = Array.isArray(details)
                ? details.join(', ')
                : String(details);

              return `${field}: ${text}`;
            })
            .join('\n\n');

          alert('Registration failed:\n\n' + messages);
        } else {
          alert(
            err.error?.message ||
            err.error?.title ||
            'Registration failed. Please check the console.'
          );
        }
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }
}
