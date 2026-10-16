import { ComponentFixture, TestBed } from '@angular/core/testing';
import { foreclosure_request } from './foreclosure-request';

describe('ForeclosureRequest', () => {
  let component: foreclosure_request;
  let fixture: ComponentFixture<foreclosure_request>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [foreclosure_request],
    }).compileComponents();

    fixture = TestBed.createComponent(foreclosure_request);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
