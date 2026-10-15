
import { Routes } from '@angular/router';

import { foreclosure_request } from './foreclosure-request/foreclosure-request';
import { PaymentHistory } from './payment-history/payment-history';
import { ClosureRequests } from './closure-requests/closure-requests';
import { EmiPayment } from './emi-payment/emi-payment';

import { LoanApplication } from './loan-application/loan-application';
import { PendingLoans } from './pending-loans/pending-loans';
import { CustomerProfileDetails } from './customer-profile-details/customer-profile-details';
import { GenerateSanction } from './generate-sanction/generate-sanction';

export const routes: Routes = [
  { path: 'payment-history', component: PaymentHistory },
  { path: 'foreclosure-request', component: foreclosure_request },
  { path: 'closure-requests', component: ClosureRequests },
  { path: 'emi-payment/:emiScheduleId', component: EmiPayment },

  { path: '', redirectTo: '/applyLoan', pathMatch: 'full' },
  { path: 'applyLoan', component: LoanApplication },
  { path: 'pendingLoans', component: PendingLoans },
  { path: 'customer-profile/:dealId', component: CustomerProfileDetails },
  { path: 'generate-sanction', component: GenerateSanction }
];
