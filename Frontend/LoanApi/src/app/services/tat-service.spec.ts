import { TestBed } from '@angular/core/testing';
import { TatService } from './tat-service';

describe('TatService', () => {
  let service: TatService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
