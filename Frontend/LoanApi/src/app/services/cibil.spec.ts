import { TestBed } from '@angular/core/testing';
import { Cibil } from './cibil';

describe('Cibil', () => {
  let service: Cibil;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Cibil);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
