import { Routes } from '@angular/router';
import { ForeclosureRequest } from './foreclosure-request/foreclosure-request';
import { Loanaccount } from './loanaccount/loanaccount';
import { Emischedule } from './emischedule/emischedule';
import { Loandetails } from './loandetails/loandetails';
import { Customerloanaccounts } from './costomerloanacc/costomerloanacc';
export const routes: Routes = [ 
  {
  path: 'foreclosure-request',
  component: ForeclosureRequest,
  },
  {
  path: 'loanaccount',
  component: Loanaccount
  },
  {
  path: 'emischedule',
  component: Emischedule
  },
  {
    path: 'loanaccount/details/:id',
    component: Loandetails
  },
   {
    path: 'costomerloanacc',
    component: Customerloanaccounts
  },

];
