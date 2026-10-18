import { TestBed } from '@angular/core/testing';
import { Loanservice } from './loanservice';

describe('Loanservice', () => {
  let service: Loanservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Loanservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
