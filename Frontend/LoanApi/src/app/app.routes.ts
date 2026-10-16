import { Routes } from '@angular/router';
import { ForeclosureRequest } from './foreclosure-request/foreclosure-request';
import { LoanApplication } from './loan-application/loan-application';
import { PendingLoans } from './pending-loans/pending-loans';
import { CustomerProfileDetails } from './customer-profile-details/customer-profile-details';
import { GenerateSanction } from './generate-sanction/generate-sanction';
import { CibilReport } from './cibil-report/cibil-report';
import { ScoreReport } from './score-report/score-report';
import { ProductReport } from './product-report/product-report';
import { LoanProtfolioReport } from './loan-protfolio-report/loan-protfolio-report';
import { EmiReport } from './emi-report/emi-report';
import { DisburseReport } from './disburse-report/disburse-report';

export const routes: Routes = [{

  path: 'foreclosure-request',
  component: ForeclosureRequest

},
{ path: '', redirectTo: '/applyLoan', pathMatch: 'full' },
{ path: 'applyLoan', component: LoanApplication },
{ path: 'pendingLoans', component: PendingLoans },
{ path: 'customer-profile/:dealId', component: CustomerProfileDetails },
{ path: 'generate-sanction', component: GenerateSanction },
{ path: 'cibil-report', component: CibilReport },
{ path: 'score-report', component: ScoreReport },
{ path: 'product-report', component: ProductReport },
{ path: 'loan-protfolio', component: LoanProtfolioReport },
{ path: 'emi-report', component: EmiReport },
{ path: 'disburse-report', component: DisburseReport }



];
