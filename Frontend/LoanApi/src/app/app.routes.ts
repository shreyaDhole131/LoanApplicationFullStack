
import { Routes } from '@angular/router';
import { foreclosure_request } from './foreclosure-request/foreclosure-request';
import { PaymentHistory } from './payment-history/payment-history';
import { ClosureRequests } from './closure-requests/closure-requests';
import { EmiPayment } from './emi-payment/emi-payment';

export const routes: Routes = [
  { path: 'payment-history', component: PaymentHistory },
  { path: 'foreclosure-request', component: foreclosure_request },
  { path: 'closure-requests', component: ClosureRequests },
  { path: 'emi-payment/:emiScheduleId', component: EmiPayment }
];
