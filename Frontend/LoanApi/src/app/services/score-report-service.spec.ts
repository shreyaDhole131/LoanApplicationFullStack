import { TestBed } from '@angular/core/testing';
import { ScoreReportService } from './score-report-service';

describe('ScoreReportService', () => {
  let service: ScoreReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ScoreReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
