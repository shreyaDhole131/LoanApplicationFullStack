import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class EligibilityService {
    http = inject(HttpClient);

    url = "http://localhost:5095/api/Eligibility";

    getHeaders() {
        const token = typeof localStorage !== 'undefined'
            ? localStorage.getItem('token')
            : null;
        return new HttpHeaders({
            Authorization: token ? `Bearer ${token}` : ''
        });
    }

    ProcessAutomaticEligibility() {
        return this.http.post<any>(
            `${this.url}/process-automatic`,
            null,
            { headers: this.getHeaders() }
        );
    }

    GetManualReviewCustomers() {
        return this.http.get<any>(
            `${this.url}/manual-review-customers`,
            { headers: this.getHeaders() }
        );
    }

    EvaluateEligibility(customerId: number) {
        return this.http.post<any>(
            `${this.url}/evaluate/${customerId}`,
            null,
            { headers: this.getHeaders() }
        );
    }

    ApproveEligibility(customerId: number) {
        return this.http.post<any>(
            `${this.url}/approve/${customerId}`,
            null,
            { headers: this.getHeaders() }
        );
    }

    RejectEligibility(
        customerId: number,
        rejectionReason: string,
        otherReason: string
    ) {
        return this.http.post<any>(
            `${this.url}/reject/${customerId}`,
            {
                rejectionReason: rejectionReason,
                otherReason: otherReason
            },
            { headers: this.getHeaders() }
        );
    }

    GetMyResult() {
        return this.http.get<any>(
            `${this.url}/my-result`,
            { headers: this.getHeaders() }
        );
    }
}