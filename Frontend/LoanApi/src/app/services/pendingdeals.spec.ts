import { TestBed } from '@angular/core/testing';
import { Pendingdeals } from './services/pendingdeals';

describe('Pendingdeals', () => {
  let service: Pendingdeals;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Pendingdeals);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
