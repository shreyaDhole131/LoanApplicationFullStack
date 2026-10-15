import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmiReport } from './emi-report';

describe('EmiReport', () => {
  let component: EmiReport;
  let fixture: ComponentFixture<EmiReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmiReport],
    }).compileComponents();

    fixture = TestBed.createComponent(EmiReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
