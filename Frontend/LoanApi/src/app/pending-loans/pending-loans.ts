import { ChangeDetectorRef, Component } from '@angular/core';
import { Pendingdeals } from '../services/pendingdeals';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';


@Component({
  selector: 'app-pending-loans',
  imports: [CommonModule, RouterLink],
  templateUrl: './pending-loans.html',
  styleUrl: './pending-loans.css',
})
export class PendingLoans {
  constructor(private service: Pendingdeals, private cdr: ChangeDetectorRef) {
    this.getAllPendingDeals();
  }
  pendingdeals: any;
  getAllPendingDeals() {
    this.service.fetchPendingDeals().subscribe({
      next: res => {
        console.log(res);
        this.pendingdeals = res;
        this.cdr.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }




}
