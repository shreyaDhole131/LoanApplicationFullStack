import { isPlatformBrowser } from '@angular/common';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

import {
  Injectable,
  inject,
  PLATFORM_ID
} from '@angular/core';


@Injectable({
  providedIn: 'root'
})
export class Notificationservice {

  private http = inject(HttpClient);

  private platformId = inject(PLATFORM_ID);


  private url =
    'https://localhost:7277/api/NotificationService';


  

  private getHeaders(): HttpHeaders {

    if (!isPlatformBrowser(this.platformId)) {

      return new HttpHeaders();

    }


    const token =
      localStorage.getItem('token');


    console.log(
      'Notification API Token:',
      token
    );


    if (!token) {

      console.warn(
        'JWT token not found in localStorage'
      );

      return new HttpHeaders();

    }


    return new HttpHeaders({

      Authorization:
        `Bearer ${token}`

    });

  }




  getNotifications() {

    return this.http.get<any[]>(

      `${this.url}/customer`,

      {
        headers: this.getHeaders()
      }

    );

  }



  getUnreadCount() {

    return this.http.get<number>(

      `${this.url}/unread-count`,

      {
        headers: this.getHeaders()
      }

    );

  }



  markAsRead(notificationId: number) {

    console.log(
      'Marking notification as read:',
      notificationId
    );


    return this.http.put(

      `${this.url}/mark-read/${notificationId}`,

      {},

      {
        headers: this.getHeaders()
      }

    );

  }

}