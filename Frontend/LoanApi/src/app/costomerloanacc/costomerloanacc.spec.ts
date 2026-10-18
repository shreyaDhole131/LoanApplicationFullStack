import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Costomerloanacc } from './costomerloanacc';

describe('Costomerloanacc', () => {
  let component: Costomerloanacc;
  let fixture: ComponentFixture<Costomerloanacc>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Costomerloanacc],
    }).compileComponents();

    fixture = TestBed.createComponent(Costomerloanacc);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
