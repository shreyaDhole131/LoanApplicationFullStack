import { TestBed } from '@angular/core/testing';
import { CustomerLoans } from './customer-loans';

describe('CustomerLoans', () => {
  let service: CustomerLoans;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CustomerLoans);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
