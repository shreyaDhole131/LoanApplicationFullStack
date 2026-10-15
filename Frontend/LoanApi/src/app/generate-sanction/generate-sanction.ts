import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { error } from 'console';
import { SanctionService } from '../services/sanction-service';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
declare var $: any;

@Component({
  selector: 'app-generate-sanction',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './generate-sanction.html',
  styleUrl: './generate-sanction.scss',
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




}
