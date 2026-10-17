import { Routes } from '@angular/router';
import { ForeclosureRequest } from './foreclosure-request/foreclosure-request';
import { Cibil } from './cibil/cibil';
import { Scorecard } from './scorecard/scorecard';
import { Eligibility } from './eligibility/eligibility';
import { EligibilityResult } from './eligibility-result/eligibility-result';

export const routes: Routes = [
  {
    path: 'foreclosure-request',
    component: ForeclosureRequest
  },
  {path: 'cibil', component: Cibil},
  {path: 'scorecard',component: Scorecard},
  {path: 'eligibility', component: Eligibility},
  {path: 'eligibility-result', component: EligibilityResult}
];