import { Component } from '@angular/core';
import { Officer } from '../officer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-support-tickets',
  imports: [Sidebar],
  templateUrl: './support-tickets.html',
  styleUrl: './support-tickets.css'
})
export class SupportTickets {

  tickets: any[] = [];

  selectedTicket: any = null;


  // =========================================================
  // CONSTRUCTOR
  // =========================================================

  constructor(private service: Officer) {

    this.getTickets();

  }


  // =========================================================
  // GET ALL SUPPORT TICKETS
  // =========================================================

  getTickets() {

    this.service.getAllSupportTickets().subscribe({

      next: (res: any[]) => {

        console.log('SUPPORT TICKETS', res);

        this.tickets = res;

      },

      error: (err: any) => {

        console.log('GET TICKETS ERROR', err);

      }

    });

  }


  // =========================================================
  // VIEW TICKET DETAILS
  // =========================================================

  viewTicket(ticketId: number) {

    this.service.getSupportTicket(ticketId).subscribe({

      next: (res: any) => {

        console.log('SUPPORT TICKET', res);

        this.selectedTicket = res;

      },

      error: (err: any) => {

        console.log('GET TICKET ERROR', err);

      }

    });

  }


  // =========================================================
  // VIEW ATTACHMENT
  // =========================================================

  viewAttachment(ticketId: number) {

    console.log('BUTTON CLICKED');

    console.log('TICKET ID:', ticketId);


    // Open a blank tab immediately.
    // This prevents browser popup blocking.

    const newTab = window.open('', '_blank');


    if (!newTab) {

      alert('Please allow pop-ups for this website.');

      return;

    }


    // Call backend

    this.service.viewSupportAttachment(ticketId).subscribe({

      next: (file: Blob) => {

        console.log('FILE RECEIVED:', file);

        console.log('FILE TYPE:', file.type);

        console.log('FILE SIZE:', file.size);


        // Create temporary URL for Blob

        const fileUrl = URL.createObjectURL(file);


        // Open file inside already opened tab

        newTab.location.href = fileUrl;


        // Release Blob URL after 1 minute

        setTimeout(() => {

          URL.revokeObjectURL(fileUrl);

        }, 60000);

      },


      error: (err: any) => {

        console.log('VIEW ATTACHMENT ERROR:', err);

        newTab.close();

        alert('Unable to view document');

      }

    });

  }


  // =========================================================
  // RESOLVE TICKET
  // =========================================================

  resolveTicket() {

    if (!this.selectedTicket) {

      return;

    }


    this.service.resolveSupportTicket(
      this.selectedTicket.ticketId
    ).subscribe({

      next: () => {

        alert('Support ticket resolved successfully');

        // Refresh ticket list

        this.getTickets();

        // Go back to ticket list

        this.selectedTicket = null;

      },

      error: (err: any) => {

        console.log('RESOLVE TICKET ERROR', err);

      }

    });

  }


  // =========================================================
  // BACK TO TICKET LIST
  // =========================================================

  backToTickets() {

    this.selectedTicket = null;

  }

}