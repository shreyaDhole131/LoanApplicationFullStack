import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OverdueReport } from './overdue-report';

describe('OverdueReport', () => {
  let component: OverdueReport;
  let fixture: ComponentFixture<OverdueReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OverdueReport],
    }).compileComponents();

    fixture = TestBed.createComponent(OverdueReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
