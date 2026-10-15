import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ScoreReport } from './score-report';

describe('ScoreReport', () => {
  let component: ScoreReport;
  let fixture: ComponentFixture<ScoreReport>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScoreReport],
    }).compileComponents();

    fixture = TestBed.createComponent(ScoreReport);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
