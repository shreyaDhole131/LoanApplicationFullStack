import { TestBed } from '@angular/core/testing';
import { CibilReportService } from './cibil-reportService';

describe('CibilReportService', () => {
  let service: CibilReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CibilReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
