import { TestBed } from '@angular/core/testing';
import { customer_loans } from './customer-loans';

describe('CustomerLoans', () => {
  let service: customer_loans;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(customer_loans);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
