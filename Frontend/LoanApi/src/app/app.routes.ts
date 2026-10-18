import { Routes } from '@angular/router';

// import { ForeclosureRequest } from './foreclosure-request/foreclosure-request';
import { Loanaccount } from './loanaccount/loanaccount';
import { Emischedule } from './emischedule/emischedule';
import { Loandetails } from './loandetails/loandetails';
import { Customerloanaccounts } from './costomerloanacc/costomerloanacc';

import { Register } from './auth/register/register';
import { Login } from './auth/login/login';
import { Otp } from './auth/otp/otp';
import { VerifyLoginOtp } from './auth/verify-login-otp/verify-login-otp';
import { Setup2fa } from './auth/setup-2fa/setup-2fa';

import { Dashboard } from './customer/dashboard/dashboard';
import { Profile } from './customer/profile/profile';
import { Kyc } from './customer/kyc/kyc';
import { Support } from './customer/support/support';

import { Dashboard as OfficerDashboard } from './officer/dashboard/dashboard';
import { Customers } from './officer/customers/customers';
import { KycVerification } from './officer/kyc-verification/kyc-verification';
import { SupportTickets } from './officer/support-tickets/support-tickets';

import { LoanApplication } from './loan-application/loan-application';
import { PendingLoans } from './pending-loans/pending-loans';
import { CustomerProfileDetails } from './customer-profile-details/customer-profile-details';
import { GenerateSanction } from './generate-sanction/generate-sanction';

import { foreclosure_request } from './foreclosure-request/foreclosure-request';
import { PaymentHistory } from './payment-history/payment-history';
import { ClosureRequests } from './closure-requests/closure-requests';
import { EmiPayment } from './emi-payment/emi-payment';

import { CibilReport } from './cibil-report/cibil-report';
import { ScoreReport } from './score-report/score-report';
import { ProductReport } from './product-report/product-report';
import { LoanProtfolioReport } from './loan-protfolio-report/loan-protfolio-report';
import { EmiReport } from './emi-report/emi-report';
<<<<<<< HEAD
import { Setup2fa } from './auth/setup-2fa/setup-2fa';
=======
>>>>>>> 0f6a40688c82c8dc7f57919a0bc03046a0c4532f
import { DisburseReport } from './disburse-report/disburse-report';
import { OverdueReport } from './overdue-report/overdue-report';
import { PrepaymentReport } from './prepayment-report/prepayment-report';
import { TatReport } from './tat-report/tat-report';
<<<<<<< HEAD
=======

import { Cibil } from './cibil/cibil';
import { Scorecard } from './scorecard/scorecard';
import { Eligibility } from './eligibility/eligibility';
import { EligibilityResult } from './eligibility-result/eligibility-result';

>>>>>>> 0f6a40688c82c8dc7f57919a0bc03046a0c4532f

export const routes: Routes = [
  // Default and authentication
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'otp', component: Otp },
  { path: 'setup-2fa', component: Setup2fa },
  { path: 'verify-login-otp', component: VerifyLoginOtp },

  // Customer
  { path: 'customer-dashboard', component: Dashboard },
  { path: 'profile', component: Profile },
  { path: 'kyc', component: Kyc },
  { path: 'support', component: Support },


  { path: '', redirectTo: '/applyLoan', pathMatch: 'full' },
  { path: 'cibil-report', component: CibilReport },
  { path: 'score-report', component: ScoreReport },
  { path: 'product-report', component: ProductReport },
  { path: 'loan-protfolio', component: LoanProtfolioReport },
  { path: 'emi-report', component: EmiReport },
  { path: 'disburse-report', component: DisburseReport },
  { path: 'overdue-report', component: OverdueReport },
  { path: 'prepayment-report', component: PrepaymentReport },
  { path: 'tat-report', component: TatReport },

  // Officer
  { path: 'officer-dashboard', component: OfficerDashboard },
  { path: 'officer/customers', component: Customers },
  { path: 'officer/kyc-verification', component: KycVerification },
  { path: 'officer/support-tickets', component: SupportTickets },

  // Loan application
  { path: 'applyLoan', component: LoanApplication },
  { path: 'pendingLoans', component: PendingLoans },
  { path: 'customer-profile/:dealId', component: CustomerProfileDetails },
  { path: 'generate-sanction', component: GenerateSanction },

  // Payments and foreclosure
  { path: 'payment-history', component: PaymentHistory },
 { path: 'foreclosure-request', component: foreclosure_request },
  { path: 'closure-requests', component: ClosureRequests },
  { path: 'emi-payment/:emiScheduleId', component: EmiPayment },

  // Reports
  { path: 'cibil-report', component: CibilReport },
  { path: 'score-report', component: ScoreReport },
  { path: 'product-report', component: ProductReport },
  { path: 'loan-protfolio', component: LoanProtfolioReport },
  { path: 'emi-report', component: EmiReport },
  { path: 'disburse-report', component: DisburseReport },
  { path: 'overdue-report', component: OverdueReport },
  { path: 'prepayment-report', component: PrepaymentReport },
  { path: 'tat-report', component: TatReport },

  // CIBIL, ScoreCard and Eligibility
  { path: 'cibil', component: Cibil },
  { path: 'scorecard', component: Scorecard },
  { path: 'eligibility', component: Eligibility },
  { path: 'eligibility-result', component: EligibilityResult },

  // Fallback
  { path: '**', redirectTo: 'login' }
];
  // Existing loan account routes
  { path: 'loanaccount', component: Loanaccount },
  { path: 'emischedule', component: Emischedule },
  { path: 'loanaccount/details/:id', component: Loandetails },
  { path: 'costomerloanacc', component: Customerloanaccounts },

  // Fallback
  { path: '**', redirectTo: 'login' }
];

