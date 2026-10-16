import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CustomerProfileDetails } from './customer-profile-details';

describe('CustomerProfileDetails', () => {
  let component: CustomerProfileDetails;
  let fixture: ComponentFixture<CustomerProfileDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerProfileDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerProfileDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
