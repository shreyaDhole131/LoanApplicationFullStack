
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private http = inject(HttpClient);

  private baseUrl = 'https://localhost:7277/api/auth';
  url = `${this.baseUrl}/register`;
  loginUrl = `${this.baseUrl}/login`;
  otpUrl = `${this.baseUrl}/verify-otp`;
  setup2FAUrl = `${this.baseUrl}/setup-2fa`;
  setup2FAQrUrl = `${this.baseUrl}/setup-2fa/qr`;
  verify2FASetupUrl = `${this.baseUrl}/verify-2fa-setup`;
  verifyLoginOtpUrl = `${this.baseUrl}/verify-login-otp`;

  register(model: any) {
    return this.http.post<any>(
      this.url,
      model
    );
  }

  login(model: any) {
    return this.http.post<any>(
      this.loginUrl,
      model
    );
  }



  otp(model: any) {
    return this.http.post<any>(
      this.otpUrl,
      model
    );
  }



  saveChallengeToken(token: string): void {

    if (!token) {
      throw new Error('Challenge token is empty.');
    }

    sessionStorage.setItem(
      'challengeToken',
      token
    );

    console.log(
      'Challenge token saved:',
      !!sessionStorage.getItem('challengeToken')
    );
  }


  getChallengeToken(): string | null {

    return sessionStorage.getItem(
      'challengeToken'
    );
  }


  clearChallengeToken(): void {

    sessionStorage.removeItem(
      'challengeToken'
    );
  }



  private decodePayload(token: string): any {

    const parts = token.split('.');

    if (parts.length !== 3) {

      throw new Error(
        'Invalid JWT format. Please log in again.'
      );
    }

    const base64 = parts[1]
      .replace(/-/g, '+')
      .replace(/_/g, '/');

    const padded = base64.padEnd(
      Math.ceil(base64.length / 4) * 4,
      '='
    );

    try {

      return JSON.parse(
        atob(padded)
      );

    } catch {

      throw new Error(
        'Unable to read authentication token.'
      );
    }
  }


 
  getChallengeHeaders(): HttpHeaders {

    const token =
      this.getChallengeToken();

    if (!token) {

      throw new Error(
        'Challenge token missing. Please log in again.'
      );
    }

    const payload =
      this.decodePayload(token);



    if (
      !payload.exp ||
      payload.exp * 1000 <= Date.now()
    ) {

      this.clearChallengeToken();

      throw new Error(
        'Challenge token expired. Please log in again.'
      );
    }


   

    console.log(
      'Challenge token information:',
      {
        userId:
          payload[
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
          ],

        email:
          payload[
            'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
          ],

        role:
          payload[
            'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
          ],

        authStage:
          payload.auth_stage,

        expires:
          new Date(
            payload.exp * 1000
          ).toISOString()
      }
    );



    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }



   setup2FA() {
    const token = this.getChallengeToken();
    if (!token) {
      throw new Error('Challenge token missing.');
    }

    const headers = new HttpHeaders()
      .set('Authorization', 'Bearer ' + token.trim())
      .set('Content-Type', 'application/json');

    return this.http.post<any>(
      this.setup2FAUrl,
      {}, // empty body
      { headers: headers }
    );
  
  }


  

  verify2FASetup(model: any) {

    const headers =
      this.getChallengeHeaders();

    return this.http.post<any>(
      this.verify2FASetupUrl,
      model,
      {
        headers: headers
      }
    );
  }


  

   verifyLoginOtp(model: any) {
    const token = this.getChallengeToken();
    
    if (!token) {
      throw new Error('Challenge token missing. Please log in again.');
    }

    const headers = new HttpHeaders()
      .set('Authorization', `Bearer ${token}`)
      .set('Content-Type', 'application/json');

    return this.http.post<any>(
      this.verifyLoginOtpUrl,
      model,
      { headers: headers }
    );
  
  }

}
