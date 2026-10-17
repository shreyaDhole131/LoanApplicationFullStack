import { TestBed } from '@angular/core/testing';
import { EmiReportService } from './emi-report-service';

describe('EmiReportService', () => {
  let service: EmiReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmiReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
