import { ChangeDetectorRef, Component } from '@angular/core';
import { Pendingdeals } from '../services/pendingdeals';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Sidebar } from '../officer/sidebar/sidebar';


@Component({
  selector: 'app-customer-profile-details',
  imports: [CommonModule, Sidebar],
  templateUrl: './customer-profile-details.html',
  styleUrl: './customer-profile-details.css',
})
export class CustomerProfileDetails {

  constructor(private service: Pendingdeals, private cdr: ChangeDetectorRef, private route: ActivatedRoute) {
    const dealId = Number(this.route.snapshot.paramMap.get('dealId'));
    this.fetchCustProfile(dealId);

  }

  custprofile: any;
  fetchCustProfile(dealId: any) {
    this.service.fetchCustProfile(dealId).subscribe({
      next: res => {
        console.log(res);
        this.custprofile = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }


  reviewdeals: any;
  reviewDeals(dealId: any, status: any) {
    this.service.reviewDeal(dealId, status).subscribe({
      next: res => {
        console.log(res);
        alert(res.message);

      },
      error: err => {
        console.log(err);
      }
    });
  }
}



