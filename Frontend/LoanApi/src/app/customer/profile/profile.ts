import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Customer } from '../customer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-profile',
  imports: [FormsModule, Sidebar],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile {

  profileData: any;

  editOpen = false;

  editModel: any = {
    firstName: '',
    lastName: '',
    mobileNo: '',
    employmentType: '',
    monthlyIncome: '',
    age: 0
  };

  constructor(
    private service: Customer,
    private cd: ChangeDetectorRef
  ) {
    this.getProfile();
  }

  getProfile() {
    this.service.getProfileUrl().subscribe({
      next: res => {
        console.log('PROFILE RESPONSE', res);

        this.profileData = res;

        this.cd.detectChanges();
      },
      error: err => {
        console.log(err);
      }
    });
  }

  openEditModal() {
    this.editModel = {
      firstName: this.profileData?.firstName || '',
      lastName: this.profileData?.lastName || '',
      mobileNo: this.profileData?.mobileNo || '',
      employmentType: this.profileData?.employmentType || '',
      monthlyIncome: this.profileData?.monthlyIncome || '',
      age: this.profileData?.age || 0
    };

    this.editOpen = true;
  }

  closeEditModal() {
    this.editOpen = false;
  }

  updateProfile() {
    this.service.updateProfile(this.editModel).subscribe({
      next: (res: any) => {
        alert(res.message);

        this.closeEditModal();

        this.getProfile();
      },
      error: (err: any) => {
        console.log(err);

        alert(err.error?.message || 'Unable to update profile');
      }
    });
  }

  viewKyc(documentId: number) {
    this.service.viewKyc(documentId).subscribe({
      next: (blob: Blob) => {

        const fileUrl = URL.createObjectURL(blob);

        window.open(fileUrl, '_blank');
      },
      error: err => {
        console.log(err);

        alert('Unable to open document');
      }
    });
  }
}