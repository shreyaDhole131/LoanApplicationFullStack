import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClosureRequests } from './closure-requests';

describe('ClosureRequests', () => {
  let component: ClosureRequests;
  let fixture: ComponentFixture<ClosureRequests>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClosureRequests],
    }).compileComponents();

    fixture = TestBed.createComponent(ClosureRequests);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
