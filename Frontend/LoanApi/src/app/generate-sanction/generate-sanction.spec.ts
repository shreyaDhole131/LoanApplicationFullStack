import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GenerateSanction } from './generate-sanction';

describe('GenerateSanction', () => {
  let component: GenerateSanction;
  let fixture: ComponentFixture<GenerateSanction>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GenerateSanction],
    }).compileComponents();

    fixture = TestBed.createComponent(GenerateSanction);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
