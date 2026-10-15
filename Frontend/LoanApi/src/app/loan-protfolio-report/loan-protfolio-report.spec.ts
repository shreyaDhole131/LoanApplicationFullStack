import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoanProtfolioReport } from './loan-protfolio-report';

describe('LoanProtfolioReport', () => {
  let component: LoanProtfolioReport;
  let fixture: ComponentFixture<LoanProtfolioReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoanProtfolioReport],
    }).compileComponents();

    fixture = TestBed.createComponent(LoanProtfolioReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
