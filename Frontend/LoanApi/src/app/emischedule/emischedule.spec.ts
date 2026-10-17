import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Emischedule } from './emischedule';

describe('Emischedule', () => {
  let component: Emischedule;
  let fixture: ComponentFixture<Emischedule>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Emischedule],
    }).compileComponents();

    fixture = TestBed.createComponent(Emischedule);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
