import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Customerloanaccounts } from './costomerloanacc'; // <-- Matched the class name

describe('Customerloanaccounts', () => { // <-- Matched the class name
  let component: Customerloanaccounts; // <-- Matched the class name
  let fixture: ComponentFixture<Customerloanaccounts>; // <-- Matched the class name

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Customerloanaccounts], // <-- Matched the class name
    }).compileComponents();

    fixture = TestBed.createComponent(Customerloanaccounts); // <-- Matched the class name
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});