import { TestBed } from '@angular/core/testing';
import { Emischeduleservice } from './emischeduleservice';

describe('Emischeduleservice', () => {
  let service: Emischeduleservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Emischeduleservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
