import { Routes } from '@angular/router';
import { Register } from './auth/register/register';
import { Login } from './auth/login/login';
import{Otp} from './auth/otp/otp';
import { Dashboard } from './customer/dashboard/dashboard';
import { Profile } from './customer/profile/profile';
import { Kyc } from './customer/kyc/kyc';
import { Support } from './customer/support/support';

import { Dashboard as OfficerDashboard } from './officer/dashboard/dashboard';
import { Customers } from './officer/customers/customers';
import { KycVerification } from './officer/kyc-verification/kyc-verification';
import { SupportTickets } from './officer/support-tickets/support-tickets';
import { VerifyLoginOtp } from './auth/verify-login-otp/verify-login-otp';


import { Setup2FA } from './auth/setup-2fa/setup-2fa';
export const routes: Routes = [
    {path:'',redirectTo:'login',pathMatch:'full'},
    {path:'login',component: Login},
  {path:'register', component:Register},
  {path:'otp',component:Otp},
  {path:'customer-dashboard',component:Dashboard},
  {path:'profile',component: Profile},
    {path:'kyc',component: Kyc},
    { path: 'support', component: Support},

{ path: 'officer-dashboard', component: OfficerDashboard },
{ path: 'officer/customers', component: Customers },
{ path: 'officer/kyc-verification', component: KycVerification},
{path: 'officer/support-tickets',component:SupportTickets},
{ path: 'setup-2fa', component: Setup2FA },
{ path: 'verify-login-otp', component: VerifyLoginOtp }
];