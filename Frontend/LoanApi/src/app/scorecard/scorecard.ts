import {
    ChangeDetectorRef,
    Component,
    Inject,
    OnInit,
    PLATFORM_ID
} from '@angular/core';

import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ScorecardService } from '../services/scorecard';
import { Sidebar } from '../officer/sidebar/sidebar';

@Component({
    selector: 'app-scorecard',
    imports: [CommonModule,Sidebar],
    templateUrl: './scorecard.html',
    styleUrl: './scorecard.css'
})
export class Scorecard implements OnInit {

    customers: any[] = [];
    selectedCustomer: any = null;
    scoreCard: any = null;

    loadingCustomers = false;

    generatingCustomerId: number | null = null;

    showDetails = false;

    errorMessage = '';
    successMessage = '';

    constructor(
        private service: ScorecardService,
        private cd: ChangeDetectorRef,
        @Inject(PLATFORM_ID) private platformId: object
    ) { }

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            this.GetCustomers();
        }
    }

    hasToken(): boolean {
        if (!isPlatformBrowser(this.platformId)) {
            return false;
        }

        const token = localStorage.getItem('token');

        if (!token) {
            this.errorMessage =
                'Officer JWT token not found. Please log in as an officer.';

            return false;
        }

        return true;
    }

    GetCustomers() {
        if (!this.hasToken()) {
            return;
        }

        this.loadingCustomers = true;
        this.errorMessage = '';

        this.service.GetCustomers().subscribe({
            next: res => {
                this.customers = res?.data ?? res?.Data ?? [];

                if (!Array.isArray(this.customers)) {
                    this.customers = [];
                }

                if (
                    res?.success === false ||
                    res?.Success === false
                ) {
                    this.customers = [];

                    this.errorMessage =
                        res?.message ??
                        res?.Message ??
                        'Unable to load customers.';
                }

                this.loadingCustomers = false;

                this.cd.detectChanges();
            },

            error: err => {
                console.log(err);

                this.loadingCustomers = false;

                if (err.status === 401) {
                    this.errorMessage =
                        'Unauthorized. Please use a valid Officer JWT token.';
                }
                else if (err.status === 403) {
                    this.errorMessage =
                        'Access denied. Only an officer can view this list.';
                }
                else {
                    this.errorMessage =
                        err.error?.message ??
                        err.error?.Message ??
                        'Unable to load customers. Check the backend connection.';
                }

                this.cd.detectChanges();
            }
        });
    }

    ViewScoreCard(customer: any) {
        if (!this.hasToken()) {
            return;
        }

        this.generatingCustomerId = customer.customerId;

        this.errorMessage = '';
        this.successMessage = '';

        this.cd.detectChanges();

        this.service.GenerateScoreCard(customer.customerId).subscribe({
            next: res => {
                const data = res?.data ?? res?.Data;

                if (
                    res?.success === false ||
                    res?.Success === false ||
                    !data
                ) {
                    this.errorMessage =
                        res?.message ??
                        res?.Message ??
                        'ScoreCard generation failed.';

                    this.generatingCustomerId = null;

                    this.cd.detectChanges();

                    return;
                }

                this.selectedCustomer = customer;
                this.scoreCard = data;
                this.showDetails = true;

                this.successMessage =
                    res?.message ??
                    res?.Message ??
                    'ScoreCard generated successfully.';

                this.generatingCustomerId = null;

                this.cd.detectChanges();
            },

            error: err => {
                console.log(err);

                if (err.status === 401) {
                    this.errorMessage =
                        'Unauthorized. Please log in again.';
                }
                else if (err.status === 403) {
                    this.errorMessage =
                        'Access denied. Please use an Officer JWT token.';
                }
                else if (err.status === 404) {
                    this.errorMessage =
                        'Customer or CIBIL report not found. Generate the CIBIL report first.';
                }
                else {
                    this.errorMessage =
                        err.error?.message ??
                        err.error?.Message ??
                        'Unable to generate the ScoreCard.';
                }

                this.generatingCustomerId = null;

                this.cd.detectChanges();
            }
        });
    }

    BackToCustomers() {
        this.showDetails = false;

        this.selectedCustomer = null;
        this.scoreCard = null;

        this.successMessage = '';
        this.errorMessage = '';

        this.generatingCustomerId = null;

        this.cd.detectChanges();
    }
}