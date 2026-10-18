import { TestBed } from '@angular/core/testing';
import { PrepaymentService } from './prepayment-service';

describe('PrepaymentService', () => {
  let service: PrepaymentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PrepaymentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
