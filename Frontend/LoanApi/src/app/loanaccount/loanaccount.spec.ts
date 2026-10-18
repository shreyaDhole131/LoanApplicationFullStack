import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Loanaccount } from './loanaccount';

describe('Loanaccount', () => {
  let component: Loanaccount;
  let fixture: ComponentFixture<Loanaccount>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Loanaccount]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Loanaccount);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
