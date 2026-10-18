import { TestBed } from '@angular/core/testing';
import { CibilService } from './cibil';

describe('CibilService', () => {
  let service: CibilService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CibilService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});