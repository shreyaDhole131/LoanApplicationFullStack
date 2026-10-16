import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmiPayment } from './emi-payment';

describe('EmiPayment', () => {
  let component: EmiPayment;
  let fixture: ComponentFixture<EmiPayment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmiPayment],
    }).compileComponents();

    fixture = TestBed.createComponent(EmiPayment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
