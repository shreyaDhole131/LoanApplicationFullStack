import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Notificationservice } from '../services/notificationservice';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.html'
})
export class NotificationComponent implements OnInit {

  private notificationService = inject(Notificationservice);

  notifications: any[] = [];

  unreadCount = 0;

  showDropdown = false;


  ngOnInit(): void {

    this.loadNotifications();

    this.loadUnreadCount();

  }


  // --------------------------------------------------
  // Open / Close notification dropdown
  // --------------------------------------------------

  toggleDropdown(): void {

    this.showDropdown = !this.showDropdown;

    if (this.showDropdown) {

      this.loadNotifications();

      this.loadUnreadCount();

    }

  }


  // --------------------------------------------------
  // Load notifications
  // --------------------------------------------------

  loadNotifications(): void {

    this.notificationService
      .getNotifications()
      .subscribe({

        next: (response) => {

          console.log(
            'Notifications:',
            response
          );

          this.notifications = response ?? [];

        },

        error: (error) => {

          console.error(
            'Notification loading error:',
            error
          );

          this.notifications = [];

        }

      });

  }


  // --------------------------------------------------
  // Load unread notification count
  // --------------------------------------------------

  loadUnreadCount(): void {

    this.notificationService
      .getUnreadCount()
      .subscribe({

        next: (count) => {

          console.log(
            'Unread count from API:',
            count
          );

          this.unreadCount = count ?? 0;

        },

        error: (error) => {

          console.error(
            'Unread count error:',
            error
          );

        }

      });

  }


  // --------------------------------------------------
  // Mark notification as read
  // --------------------------------------------------

  markAsRead(notificationId: number): void {

    console.log(
      'Clicked notification ID:',
      notificationId
    );


    this.notificationService
      .markAsRead(notificationId)
      .subscribe({

        next: (response) => {

          console.log(
            'Mark as read response:',
            response
          );


          // ------------------------------------------
          // Remove notification immediately from UI
          // ------------------------------------------

          this.notifications =
            this.notifications.filter(
              item =>
                item.notificationId !== notificationId
            );


          // ------------------------------------------
          // Decrease bell count immediately
          // ------------------------------------------

          if (this.unreadCount > 0) {

            this.unreadCount--;

          }


          // ------------------------------------------
          // Synchronize count with database
          // ------------------------------------------

          this.loadUnreadCount();


          // ------------------------------------------
          // Close dropdown after reading
          // ------------------------------------------

          this.showDropdown = false;

        },

        error: (error) => {

          console.error(
            'Mark notification as read error:',
            error
          );

        }

      });

  }

}