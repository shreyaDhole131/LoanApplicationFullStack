import { TestBed } from '@angular/core/testing';
import { DisbursementReportService } from './disbursement-report-service';

describe('DisbursementReportService', () => {
  let service: DisbursementReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DisbursementReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
