import {
    ChangeDetectorRef,
    Component,
    Inject,
    OnInit,
    PLATFORM_ID
} from '@angular/core';

import { CommonModule, isPlatformBrowser } from '@angular/common';
import { EligibilityService } from '../services/eligibility';
import { Sidebar } from '../officer/sidebar/sidebar';


@Component({
    selector: 'app-eligibility-result',
    imports: [CommonModule,Sidebar],
    templateUrl: './eligibility-result.html',
    styleUrl: './eligibility-result.css'
})
export class EligibilityResult implements OnInit {
    result: any = null;
    loading = false;
    errorMessage = '';

    constructor(
        private service: EligibilityService,
        private cd: ChangeDetectorRef,
        @Inject(PLATFORM_ID) private platformId: object
    ) { }

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            this.GetMyResult();
        }
    }

    GetMyResult() {
        if (!isPlatformBrowser(this.platformId)) return;

        const token = localStorage.getItem('token');

        if (!token) {
            this.result = null;
            this.errorMessage =
                'Customer token not found. Please log in as a customer.';
            return;
        }

        this.loading = true;
        this.errorMessage = '';

        this.service.GetMyResult().subscribe({
            next: res => {
                const data = res?.data ?? res?.Data;

                if (
                    res?.success === false ||
                    res?.Success === false ||
                    !data
                ) {
                    this.result = null;
                    this.errorMessage =
                        res?.message ?? res?.Message ??
                        'No eligibility result is available.';
                } else {
                    this.result = data;
                }

                this.loading = false;
                this.cd.detectChanges();
            },

            error: err => {
                console.log(err);

                this.loading = false;
                this.result = null;

                if (err.status === 401) {
                    this.errorMessage =
                        'Your session has expired. Please log in again.';
                } else if (err.status === 403) {
                    this.errorMessage =
                        'Access denied. Please log in with a Customer account.';
                } else if (err.status === 404) {
                    this.errorMessage =
                        'Your eligibility result has not been generated yet.';
                } else {
                    this.errorMessage =
                        err.error?.message ??
                        err.error?.Message ??
                        'Unable to load the eligibility result.';
                }

                this.cd.detectChanges();
            }
        });
    }

    GetStatus(): string {
        return (this.result?.applicationStatus ?? 'Pending')
            .toString()
            .trim()
            .toLowerCase();
    }

    IsApproved(): boolean {
        const status = this.GetStatus();

        return status === 'approved' ||
            status === 'auto approved';
    }

    IsRejected(): boolean {
        const status = this.GetStatus();

        return status === 'rejected' ||
            status === 'auto rejected';
    }

    IsPending(): boolean {
        const status = this.GetStatus();

        return status === 'pending' ||
            status === 'manual review';
    }

    GetStatusClass(): string {
        if (this.IsApproved()) {
            return 'approved';
        }

        if (this.IsRejected()) {
            return 'rejected';
        }

        return 'pending';
    }

    GetStatusTitle(): string {
        const status = this.result?.applicationStatus;

        if (!status) {
            return 'Pending';
        }

        return status;
    }

    GetStatusMessage(): string {
        const status = this.GetStatus();

        if (status === 'auto approved') {
            return 'Your application passed the automatic eligibility checks.';
        }

        if (status === 'approved') {
            return 'Your loan eligibility has been approved by the loan officer.';
        }

        if (status === 'auto rejected') {
            return 'Your application did not meet the automatic eligibility criteria.';
        }

        if (status === 'rejected') {
            return 'Your loan eligibility application has been rejected.';
        }

        if (status === 'manual review') {
            return 'Your application is awaiting a loan officer’s decision.';
        }

        return 'Your eligibility application is awaiting processing.';
    }
}