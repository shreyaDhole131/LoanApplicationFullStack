import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TatReport } from './tat-report';

describe('TatReport', () => {
  let component: TatReport;
  let fixture: ComponentFixture<TatReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TatReport],
    }).compileComponents();

    fixture = TestBed.createComponent(TatReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
