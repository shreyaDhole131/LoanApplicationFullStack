import {
  Component,
  ChangeDetectorRef
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import { Customer } from '../customer';

import { Sidebar } from '../sidebar/sidebar';


@Component({
  selector: 'app-kyc',

  imports: [
    FormsModule,Sidebar 
  ],

  templateUrl: './kyc.html',

  styleUrl: './kyc.css'
})
export class Kyc {

  documents: any[] = [];

  selectedDocumentType = '';

  selectedFile: File | null = null;

  uploading = false;


  constructor(
    private service: Customer,

    private cd: ChangeDetectorRef
  ) {

    this.getKycDocuments();

  }


  // GET DOCUMENTS

  getKycDocuments() {

    this.service
      .getMyKycDocuments()
      .subscribe({

        next: res => {

          console.log(
            "KYC RESPONSE",
            res
          );

          this.documents = res;

          this.cd.detectChanges();

        },

        error: err => {

          console.log(err);

        }

      });

  }


  // SELECT FILE

  onFileSelected(event: any) {

    const file =
      event.target.files[0];


    if (file) {

      this.selectedFile = file;

      console.log(
        "Selected File:",
        file.name
      );

    }

  }


  // UPLOAD

  uploadDocument() {

    if (!this.selectedDocumentType) {

      alert(
        'Please select document type'
      );

      return;

    }


    if (!this.selectedFile) {

      alert(
        'Please select a file'
      );

      return;

    }


    this.uploading = true;


    this.service
      .uploadKyc(
        this.selectedDocumentType,
        this.selectedFile
      )
      .subscribe({

        next: res => {

          console.log(res);

          alert(res.message);


          this.selectedDocumentType =
            '';

          this.selectedFile =
            null;

          this.uploading =
            false;


          this.getKycDocuments();

        },


        error: err => {

          console.log(err);

          alert(
            err.error?.message ||
            'Unable to upload document'
          );


          this.uploading =
            false;

        }

      });

  }


  // VIEW

  viewDocument(
    documentId: number
  ) {

    this.service
      .viewKyc(documentId)
      .subscribe({

        next: blob => {

          const fileUrl =
            URL.createObjectURL(blob);


          window.open(
            fileUrl,
            '_blank'
          );

        },


        error: err => {

          console.log(err);

          alert(
            'Unable to open document'
          );

        }

      });

  }


  // DELETE

  deleteDocument(
    documentId: number
  ) {

    const confirmDelete =
      confirm(
        'Are you sure you want to delete this KYC document?'
      );


    if (!confirmDelete) {

      return;

    }


    this.service
      .deleteKyc(documentId)
      .subscribe({

        next: res => {

          console.log(res);

          alert(res.message);

          this.getKycDocuments();

        },


        error: err => {

          console.log(err);

          alert(
            err.error?.message ||
            'Unable to delete document'
          );

        }

      });

  }

}