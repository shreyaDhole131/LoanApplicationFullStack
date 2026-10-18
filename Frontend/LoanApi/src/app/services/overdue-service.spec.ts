import { TestBed } from '@angular/core/testing';
import { OverdueService } from './overdue-service';

describe('OverdueService', () => {
  let service: OverdueService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OverdueService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
