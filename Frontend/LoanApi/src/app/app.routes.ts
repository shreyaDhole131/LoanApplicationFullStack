import { Routes } from '@angular/router';
import { foreclosure_request } from './foreclosure-request/foreclosure-request';
import { PaymentHistory } from './payment-history/payment-history';

export const routes: Routes = [
  {path: 'payment-history',component: PaymentHistory}, 
  { path: 'foreclosure-request',component: foreclosure_request}];
import { LoanApplication } from './loan-application/loan-application';
import { PendingLoans } from './pending-loans/pending-loans';
import { CustomerProfileDetails } from './customer-profile-details/customer-profile-details';
import { GenerateSanction } from './generate-sanction/generate-sanction';

export const routes: Routes = [{

  path: 'foreclosure-request',
  component: ForeclosureRequest

},
{ path: '', redirectTo: '/applyLoan', pathMatch: 'full' },
{ path: 'applyLoan', component: LoanApplication },
{ path: 'pendingLoans', component: PendingLoans },
{ path: 'customer-profile/:dealId', component: CustomerProfileDetails },
{ path: 'generate-sanction', component: GenerateSanction }



];
