import { Component, ChangeDetectorRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Customer } from '../customer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-support',
  imports: [FormsModule, Sidebar, DatePipe],
  templateUrl: './support.html',
  styleUrl: './support.css'
})
export class Support {

  tickets: any[] = [];

  model: any = {
    loanAccountId: 0,
    subject: '',
    description: '',
    file: null
  };

  editMode = false;
  editTicketId = 0;
  modalOpen = false;

  constructor(
    private service: Customer,
    private cd: ChangeDetectorRef
  ) {
    this.getTickets();
  }

  openAddModal() {

    this.editMode = false;
    this.editTicketId = 0;

    this.model = {
      loanAccountId: 0,
      subject: '',
      description: '',
      file: null
    };

    this.modalOpen = true;
  }

  openEditModal(ticket: any) {

    this.editMode = true;
    this.editTicketId = ticket.ticketId;

    this.model = {
      loanAccountId: ticket.loanAccountId,
      subject: ticket.subject,
      description: ticket.description,
      file: null
    };

    this.modalOpen = true;
  }

  closeModal() {

    this.modalOpen = false;
    this.editMode = false;
    this.editTicketId = 0;

    this.model = {
      loanAccountId: 0,
      subject: '',
      description: '',
      file: null
    };
  }

  onFileSelected(event: any) {

    const file = event.target.files[0];

    if (file) {
      this.model.file = file;
    }
  }

  createTicket() {

    if (!this.model.loanAccountId) {
      alert('Please enter loan account');
      return;
    }

    if (!this.model.subject) {
      alert('Please enter subject');
      return;
    }

    if (!this.model.description) {
      alert('Please enter description');
      return;
    }

    this.service.createTicket(this.model).subscribe({

      next: (res: any) => {

        alert(res.message);

        this.closeModal();
        this.getTickets();
      },

      error: (err: any) => {

        console.log(err);

        alert(
          err.error?.message ||
          'Unable to create support ticket'
        );
      }
    });
  }

  getTickets() {

    this.service.getMyTickets().subscribe({

      next: (res: any) => {

        this.tickets = res;

        this.cd.detectChanges();
      },

      error: (err: any) => {

        console.log(err);
      }
    });
  }

  viewTicket(ticketId: number) {

    this.service.viewTicketFile(ticketId).subscribe({

      next: (blob: Blob) => {

        const fileUrl = URL.createObjectURL(blob);

        window.open(fileUrl, '_blank');
      },

      error: (err: any) => {

        console.log(err);

        alert('Unable to open attachment');
      }
    });
  }

  updateTicket() {

    if (!this.model.loanAccountId) {
      alert('Please enter loan account');
      return;
    }

    if (!this.model.subject) {
      alert('Please enter subject');
      return;
    }

    if (!this.model.description) {
      alert('Please enter description');
      return;
    }

    this.service.updateTicket(
      this.editTicketId,
      this.model
    ).subscribe({

      next: (res: any) => {

        alert(res.message);

        this.closeModal();
        this.getTickets();
      },

      error: (err: any) => {

        console.log(err);

        alert(
          err.error?.message ||
          'Unable to update support ticket'
        );
      }
    });
  }

  deleteTicket(ticketId: number) {

    const result = confirm(
      'Are you sure you want to delete this ticket?'
    );

    if (!result)
      return;

    this.service.deleteTicket(ticketId).subscribe({

      next: (res: any) => {

        alert(res.message);

        this.getTickets();
      },

      error: (err: any) => {

        console.log(err);

        alert(
          err.error?.message ||
          'Unable to delete ticket'
        );
      }
    });
  }
}