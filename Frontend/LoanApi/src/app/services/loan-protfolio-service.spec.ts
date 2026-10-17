import { TestBed } from '@angular/core/testing';
import { LoanProtfolioService } from './loan-protfolio-service';

describe('LoanProtfolioService', () => {
  let service: LoanProtfolioService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoanProtfolioService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
