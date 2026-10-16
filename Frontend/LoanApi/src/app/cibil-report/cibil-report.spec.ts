import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CibilReport } from './cibil-report';

describe('CibilReport', () => {
  let component: CibilReport;
  let fixture: ComponentFixture<CibilReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CibilReport],
    }).compileComponents();

    fixture = TestBed.createComponent(CibilReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
