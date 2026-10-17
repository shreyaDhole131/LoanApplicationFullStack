import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PrepaymentReport } from './prepayment-report';

describe('PrepaymentReport', () => {
  let component: PrepaymentReport;
  let fixture: ComponentFixture<PrepaymentReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrepaymentReport],
    }).compileComponents();

    fixture = TestBed.createComponent(PrepaymentReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
