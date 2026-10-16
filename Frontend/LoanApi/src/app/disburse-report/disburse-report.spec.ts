import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DisburseReport } from './disburse-report';

describe('DisburseReport', () => {
  let component: DisburseReport;
  let fixture: ComponentFixture<DisburseReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DisburseReport],
    }).compileComponents();

    fixture = TestBed.createComponent(DisburseReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
