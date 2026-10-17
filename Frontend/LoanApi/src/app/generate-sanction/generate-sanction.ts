import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { error } from 'console';
import { SanctionService } from '../services/sanction-service';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Sidebar } from '../officer/sidebar/sidebar';
declare var $: any;

@Component({
  selector: 'app-generate-sanction',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Sidebar],
  templateUrl: './generate-sanction.html',
  styleUrl: './generate-sanction.css',
})
export class GenerateSanction {
  constructor(private service: SanctionService, private cd: ChangeDetectorRef) {
    this.fetchpendingcusts();
    this.GetSanctionGrid()
  }

  selectCustomerId: any;
  loanAmount: any;
  loanType: any;
  tenureMonths: any;
  interestRate: any;
  emiAmount: any;

  pendingcustomers: any;
  fetchpendingcusts() {
    this.service.fetchPendingCustomers().subscribe({
      next: res => {
        console.log(res);
        this.pendingcustomers = res;
        this.cd.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  onCustomerChange() {
    if (!this.selectCustomerId) return;

    this.service.prefillCusts(this.selectCustomerId).subscribe({
      next: (res: any) => {
        console.log(res);

        this.loanAmount = res.loanAmount;
        this.loanType = res.loanType;
        this.tenureMonths = res.tenureMonths;
        this.interestRate = res.interestRate;
        this.emiAmount = res.emiAmount;

        this.cd.detectChanges();
      },
      error: (err: any) => console.log(err)
    });
  }

  SaveSanctionLetter() {
    const payload = {
      dealId: this.selectCustomerId,
      loanAmount: this.loanAmount,
      loanType: this.loanType,
      tenureMonths: this.tenureMonths,
      interestRate: this.interestRate,
      emiAmount: this.emiAmount
    };

    this.service.AddSanctionLetter(payload).subscribe({
      next: (res: any) => {
        console.log(res);
        alert(res.message);
        $("#exampleModal").modal('hide');
      },
      error: (err: any) => {
        console.log(err);
        alert(err.error?.message || 'Failed');
      }
    });
  }

  allsanctions: any;
  GetSanctionGrid() {
    this.service.GetSanctionGrid({ name: '', fromDate: '', toDate: '', filter: '', sort: '' }).subscribe({
      next: res => {
        console.log(res);
        this.allsanctions = res;
        this.cd.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });

  }

  name = '';
  fromDate = '';
  toDate = '';
  filter = '';
  sort = '';
  applyFilters() {
    this.page = 1;
    const filters: any = {};
    if (this.name) filters.name = this.name;
    if (this.fromDate) filters.fromDate = this.fromDate;
    if (this.toDate) filters.toDate = this.toDate;
    if (this.filter) filters.filter = this.filter;
    if (this.sort) filters.sort = this.sort;

    this.service.GetSanctionGrid(filters).subscribe({
      next: res => {
        this.allsanctions = res;
        this.cd.detectChanges();
      },
      error: err => console.log(err)
    });
  }

  resetFilters() {
    this.page = 1;
    this.pageSize = 10;
    this.name = '';
    this.fromDate = '';
    this.toDate = '';
    this.filter = '';
    this.sort = '';
    this.GetSanctionGrid();
  }


  dealId: any;
  disbursemntAmount: any;
  prefilDisbursement(dealId: number) {
    if (!dealId) {
      console.error('dealId missing — check the grid DTO');
      return;
    }
    this.service.PrefilDisbursement(dealId).subscribe({
      next: (res: any) => {
        console.log(res);

        this.dealId = res.dealId;
        this.disbursementAmount = res.disburseAmount;
        this.cd.detectChanges();


      },
      error: (err: any) => console.log(err)
    });

  }


  disbursementAmount: any;
  disbursementDate: any;
  disbursementPartnerBank: any;
  disbursements: any;
  applyDisbursement() {
    const payload = {
      dealId: this.dealId,
      disburseAmount: this.disbursementAmount,
      bankPartner: this.disbursementPartnerBank,
      disbursementDate: this.disbursementDate
    };

    this.service.ApplyDisbursement(payload).subscribe({
      next: (res: any) => {
        console.log(res);
        alert(res.message);
        $("#exampleModal2").modal('hide');
        this.GetSanctionGrid();
        this.cd.detectChanges();

      },
      error: (err: any) => {
        console.log(err);
        alert(err.error?.message || 'Failed');
      }
    })
  }

  letter: any;
  letterId: any;
  GetSanctionLetter(letterId: number) {
    if (!letterId) {
      alert('No sanction letter for this deal.');
      return;
    }

    this.service.getSanctionLetter(letterId).subscribe({
      next: res => {
        console.log(res)
        this.letter = res;
      },
      error: err => {
        console.log(err);
      }
    })

  }

  openPdf(letterId: number) {
    if (!letterId) { alert('No sanction letter generated for this deal.'); return; }
    window.open(this.service.viewPdf(letterId), '_blank');
  }

  downloadPdf(letterId: number) {
    if (!letterId) { alert('No sanction letter for this deal.'); return; }
    window.open(this.service.downloadPdf(letterId), '_blank');
  }

  // pagination
  page = 1;
  pageSize = 10;

  get pagedSanctions(): any[] {
    const start = (this.page - 1) * this.pageSize;
    return (this.allsanctions || []).slice(start, start + this.pageSize);
  }

  get totalCount(): number {
    return this.allsanctions?.length || 0;
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize) || 1;
  }

  get pagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  goToPage(n: number) {
    if (n < 1 || n > this.totalPages) return;
    this.page = n;
  }

  onPageSizeChange() {
    this.page = 1;
  }




}
