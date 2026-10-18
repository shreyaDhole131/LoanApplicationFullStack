import {
  ChangeDetectorRef,
  Component,
  Inject,
  PLATFORM_ID
} from '@angular/core';

import { CommonModule, isPlatformBrowser } from '@angular/common';
import { CibilService } from '../services/cibil';

@Component({
  imports: [CommonModule],
  selector: 'app-cibil',
  styleUrl: './cibil.css',
  templateUrl: './cibil.html'
})
export class Cibil {

  customer: any = null;
  result: any = null;

  loading = false;
  generating = false;
  showScore = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private service: CibilService,
    private cd: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: object
  )
  {
    if (isPlatformBrowser(this.platformId)) {
      this.GetMyCibil();
    }
  }

  // Get the logged-in customer's saved CIBIL report
  GetMyCibil()
  {
    if (!this.hasToken())
      return;

    this.loading = true;
    this.errorMessage = '';

    this.service.GetMyCibil().subscribe({
      next: res => {
        console.log(res);

        this.result = res?.data ?? res?.Data;
        this.customer = this.result;
        this.showScore = !!this.result;
        this.loading = false;

        this.cd.detectChanges();
      },
      error: err => {
        console.log(err);

        this.loading = false;
        this.customer = null;
        this.result = null;
        this.showScore = false;

        if (err.status === 404) {
          this.errorMessage = '';
        }
        else if (err.status === 401) {
          this.errorMessage =
            'JWT token is missing or invalid. Please log in again.';
        }
        else if (err.status === 403) {
          this.errorMessage =
            'Your account does not have customer access.';
        }
        else {
          this.errorMessage =
            'Unable to load your CIBIL report. Check the backend connection.';
        }

        this.cd.detectChanges();
      }
    });
  }

  // Generate CIBIL score for the logged-in customer
  GenerateCibil()
  {
    if (!this.hasToken())
      return;

    this.generating = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.service.GenerateMyCibil().subscribe({
      next: res => {
        console.log(res);

        const data = res?.data ?? res?.Data;

        if (
          res?.success === false ||
          res?.Success === false ||
          !data
        ) {
          this.errorMessage =
            res?.message ??
            res?.Message ??
            'CIBIL score generation failed.';

          this.generating = false;
          this.cd.detectChanges();
          return;
        }

        this.result = data;
        this.customer = data;
        this.showScore = true;

        this.successMessage =
          res?.message ??
          res?.Message ??
          'CIBIL score generated successfully.';

        this.generating = false;
        this.cd.detectChanges();
      },
      error: err => {
        console.log(err);

        if (err.status === 401) {
          this.errorMessage =
            'Unauthorized. Please log in and use a valid JWT token.';
        }
        else if (err.status === 403) {
          this.errorMessage =
            'Only an authenticated customer can generate this score.';
        }
        else {
          this.errorMessage =
            err.error?.message ??
            err.error?.Message ??
            'Unable to generate the CIBIL score.';
        }

        this.generating = false;
        this.cd.detectChanges();
      }
    });
  }

  // Check whether a JWT is available
  hasToken(): boolean {
    if (!isPlatformBrowser(this.platformId)) {
        return false;
    }

    const token = localStorage.getItem('token');

    if (!token) {
        this.errorMessage =
            'JWT token not found. Please log in before checking your score.';

        return false;
    }

    return true;
}

  // Calculate the gauge needle angle
  getNeedleAngle(): number
  {
    const score = Number(this.result?.cibilScore ?? 700);
    const validScore = Math.max(500, Math.min(900, score));

    return ((validScore - 700) / 200) * 90;
  }

  // Apply colour according to score status
  getStatusClass(): string
  {
    const status = this.result?.scoreStatus;

    if (
      status === 'Excellent' ||
      status === 'Very Good' ||
      status === 'Good'
    ) {
      return 'status-good';
    }

    if (
      status === 'Average' ||
      status === 'Risky' ||
      status === 'Insufficient History'
    ) {
      return 'status-medium';
    }

    return 'status-bad';
  }
}