import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class ScorecardService {
    http = inject(HttpClient);

    url = "http://localhost:5095/api/ScoreCard/customers";

    GetCustomers() {
        return this.http.get<any>(
            this.url,
            { headers: this.getHeaders() }
        );
    }

    url2 = "http://localhost:5095/api/ScoreCard/Generate";

    GenerateScoreCard(customerId: number) {
        return this.http.post<any>(
            `${this.url2}/${customerId}`,
            null,
            { headers: this.getHeaders() }
        );
    }

    getHeaders() {
        const token = typeof localStorage !== 'undefined'
            ? localStorage.getItem('token')
            : null;

        return new HttpHeaders({
            Authorization: token ? `Bearer ${token}` : ''
        });
    }
}