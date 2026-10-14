import { Routes } from '@angular/router';
import { foreclosure_request } from './foreclosure-request/foreclosure-request';
import { PaymentHistory } from './payment-history/payment-history';

export const routes: Routes = [
  {path: 'payment-history',component: PaymentHistory}, 
  { path: 'foreclosure-request',component: foreclosure_request}];
