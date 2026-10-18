import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpHeaders } from '@angular/common/http';
import { PLATFORM_ID } from '@angular/core';

@Service()
export class Loanservice {

    http=inject(HttpClient)
    platformId = inject(PLATFORM_ID)

    url="https://localhost:7277/api/LoanAccount/getA"

    getLoanAccounts()
    {
        return this.http.get<any>(this.url);
    }

  
    url2="https://localhost:7277/api/LoanAccount/getById"
    


     getLoanAccountById(id: number) {

        let token = '';

        if (isPlatformBrowser(this.platformId)) {
            token = localStorage.getItem('token') ?? '';
        }

        const headers = new HttpHeaders({
            Authorization: `Bearer ${token}`
        });

        return this.http.get<any>(
            `${this.url2}/${id}`,
            { headers }
        );
        
    }


    url3='https://localhost:7277/api/LoanAccount';

    searchLoanAccounts(search: string) {

  return this.http.get<any[]>(
     `${this.url}/search?search=${encodeURIComponent(search)}`
  );

}


}
