import { TestBed } from '@angular/core/testing';

import { Applyloan } from './applyloan';

describe('Applyloan', () => {
  let service: Applyloan;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Applyloan);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
