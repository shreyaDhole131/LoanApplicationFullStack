import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Service } from '@angular/core';

@Service()
export class CibilService {

    http = inject(HttpClient);

    url = "http://localhost:5095/api/Cibil/GenerateMyCibil";

    GenerateMyCibil()
    {
        return this.http.post<any>(
            this.url,
            null,
            { headers: this.getHeaders() }
        );
    }

    url2 = "http://localhost:5095/api/Cibil/MyCibil";

    GetMyCibil()
    {
        return this.http.get<any>(
            this.url2,
            { headers: this.getHeaders() }
        );
    }

    getHeaders()
    {
        const token = typeof localStorage !== 'undefined'
            ? localStorage.getItem('token')
            : null;

        return new HttpHeaders({
            Authorization: token ? `Bearer ${token}` : ''
        });
    }
}