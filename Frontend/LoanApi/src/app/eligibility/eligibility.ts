import {
    ChangeDetectorRef,
    Component,
    Inject,
    OnInit,
    PLATFORM_ID
} from '@angular/core';

import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EligibilityService } from '../services/eligibility';

@Component({
    selector: 'app-eligibility',
    imports: [CommonModule, FormsModule],
    templateUrl: './eligibility.html',
    styleUrl: './eligibility.css'
})
export class Eligibility implements OnInit {
    customers: any[] = [];
    selectedCustomerId: number | null = null;
    evaluation: any = null;

    loadingCustomers = false;
    checking = false;
    decisionInProgress = false;

    rejectionReason = 'Insufficient repayment capacity';
    otherReason = '';

    errorMessage = '';
    successMessage = '';

    constructor(
        private service: EligibilityService,
        private cd: ChangeDetectorRef,
        @Inject(PLATFORM_ID) private platformId: object
    ) { }

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            this.LoadPendingCustomers();
        }
    }

    hasToken(): boolean {
        if (!isPlatformBrowser(this.platformId)) {
            return false;
        }

        const token = localStorage.getItem('token');

        if (!token) {
            this.errorMessage =
                'Officer token not found. Please log in as an Officer.';
            return false;
        }

        return true;
    }

    // Process automatic decisions, then load pending customers
    LoadPendingCustomers() {
        if (!this.hasToken()) return;

        this.loadingCustomers = true;
        this.errorMessage = '';
        this.successMessage = '';

        this.service.ProcessAutomaticEligibility().subscribe({
            next: res => {
                if (res?.success === false || res?.Success === false) {
                    this.errorMessage =
                        res?.message ?? res?.Message ??
                        'Automatic eligibility processing failed.';

                    this.loadingCustomers = false;
                    this.cd.detectChanges();
                    return;
                }

                this.GetCustomersOnly();
            },
            error: err => {
                console.log(err);

                this.loadingCustomers = false;
                this.errorMessage = this.GetErrorMessage(
                    err,
                    'Unable to process eligibility.'
                );

                this.cd.detectChanges();
            }
        });
    }

    GetCustomersOnly() {
        this.service.GetManualReviewCustomers().subscribe({
            next: res => {
                this.customers = res?.data ?? res?.Data ?? [];

                if (!Array.isArray(this.customers)) {
                    this.customers = [];
                }

                this.loadingCustomers = false;
                this.cd.detectChanges();
            },
            error: err => {
                console.log(err);

                this.customers = [];
                this.loadingCustomers = false;
                this.errorMessage = this.GetErrorMessage(
                    err,
                    'Unable to load pending customers.'
                );

                this.cd.detectChanges();
            }
        });
    }

    // Reset previously displayed details when another customer is selected
    OnCustomerChange() {
        this.evaluation = null;
        this.errorMessage = '';
        this.successMessage = '';
        this.otherReason = '';
    }

    // Preview the selected customer's eligibility
    CheckEligibility() {
        if (this.selectedCustomerId == null) {
            this.errorMessage = 'Please select a customer.';
            return;
        }

        this.checking = true;
        this.errorMessage = '';
        this.successMessage = '';
        this.evaluation = null;

        this.service.EvaluateEligibility(
            this.selectedCustomerId
        ).subscribe({
            next: res => {
                const data = res?.data ?? res?.Data;

                if (
                    res?.success === false ||
                    res?.Success === false ||
                    !data
                ) {
                    this.errorMessage =
                        res?.message ?? res?.Message ??
                        'Unable to evaluate this customer.';

                    this.checking = false;
                    this.cd.detectChanges();
                    return;
                }

                this.evaluation = data;
                this.checking = false;
                this.cd.detectChanges();
            },
            error: err => {
                console.log(err);

                this.errorMessage = this.GetErrorMessage(
                    err,
                    'Unable to evaluate eligibility.'
                );

                this.checking = false;
                this.cd.detectChanges();
            }
        });
    }

    // Approve the selected customer
    ApproveEligibility() {
        if (this.selectedCustomerId == null) return;

        this.decisionInProgress = true;
        this.errorMessage = '';
        this.successMessage = '';

        this.service.ApproveEligibility(
            this.selectedCustomerId
        ).subscribe({
            next: res => {
                const data = res?.data ?? res?.Data;

                if (
                    res?.success === false ||
                    res?.Success === false ||
                    !data
                ) {
                    this.errorMessage =
                        res?.message ?? res?.Message ??
                        'Approval failed.';

                    this.decisionInProgress = false;
                    this.cd.detectChanges();
                    return;
                }

                this.evaluation = data;
                this.successMessage =
                    res?.message ?? res?.Message ??
                    'Customer approved successfully.';

                this.RemoveProcessedCustomer();
                this.decisionInProgress = false;
                this.cd.detectChanges();
            },
            error: err => {
                console.log(err);

                this.errorMessage = this.GetErrorMessage(
                    err,
                    'Unable to approve the customer.'
                );

                this.decisionInProgress = false;
                this.cd.detectChanges();
            }
        });
    }

    // Reject the selected customer
    RejectEligibility() {
        if (this.selectedCustomerId == null) return;

        const finalReason =
            this.rejectionReason === 'Other'
                ? this.otherReason.trim()
                : this.rejectionReason.trim();

        if (!finalReason) {
            this.errorMessage = 'Please enter a rejection reason.';
            return;
        }

        this.decisionInProgress = true;
        this.errorMessage = '';
        this.successMessage = '';

        this.service.RejectEligibility(
            this.selectedCustomerId,
            this.rejectionReason,
            this.otherReason
        ).subscribe({
            next: res => {
                const data = res?.data ?? res?.Data;

                if (
                    res?.success === false ||
                    res?.Success === false ||
                    !data
                ) {
                    this.errorMessage =
                        res?.message ?? res?.Message ??
                        'Rejection failed.';

                    this.decisionInProgress = false;
                    this.cd.detectChanges();
                    return;
                }

                this.evaluation = data;
                this.successMessage =
                    res?.message ?? res?.Message ??
                    'Customer rejected successfully.';

                this.RemoveProcessedCustomer();
                this.decisionInProgress = false;
                this.cd.detectChanges();
            },
            error: err => {
                console.log(err);

                this.errorMessage = this.GetErrorMessage(
                    err,
                    'Unable to reject the customer.'
                );

                this.decisionInProgress = false;
                this.cd.detectChanges();
            }
        });
    }

    RemoveProcessedCustomer() {
        if (this.selectedCustomerId == null) return;

        this.customers = this.customers.filter(
            c => c.customerId !== this.selectedCustomerId
        );

        this.selectedCustomerId = null;
    }

    ResetEvaluation() {
        this.evaluation = null;
        this.selectedCustomerId = null;
        this.errorMessage = '';
        this.successMessage = '';
        this.otherReason = '';
    }

    GetErrorMessage(err: any, defaultMessage: string): string {
        if (err.status === 401) {
            return 'Unauthorized. Please log in again.';
        }

        if (err.status === 403) {
            return 'Access denied. Please use the correct role JWT.';
        }

        return err.error?.message ??
            err.error?.Message ??
            defaultMessage;
    }
}