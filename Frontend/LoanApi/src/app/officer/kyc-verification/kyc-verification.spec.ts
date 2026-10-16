import { ComponentFixture, TestBed } from '@angular/core/testing';
import { KycVerification } from './kyc-verification';

describe('KycVerification', () => {
  let component: KycVerification;
  let fixture: ComponentFixture<KycVerification>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KycVerification],
    }).compileComponents();

    fixture = TestBed.createComponent(KycVerification);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
