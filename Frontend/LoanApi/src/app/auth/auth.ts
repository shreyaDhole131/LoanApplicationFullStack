
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class Auth {

  http = inject(HttpClient);

  url = "https://localhost:7277/api/auth/register";
  loginUrl = "https://localhost:7277/api/auth/login";
  otpUrl = "https://localhost:7277/api/auth/verify-otp";

  setup2FAUrl = "https://localhost:7277/api/auth/setup-2fa";
  get2FAQrUrl = "https://localhost:7277/api/auth/setup-2fa/qr";
  verify2FASetupUrl = "https://localhost:7277/api/auth/verify-2fa-setup";

  register(model: any) {
    return this.http.post<any>(this.url, model);
  }

  login(model: any) {
    return this.http.post<any>(this.loginUrl, model);
  }

  otp(model: any) {
    return this.http.post<any>(this.otpUrl, model);
  }

  getChallengeHeaders() {
    const token = sessionStorage.getItem('challengeToken') || '';

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  setup2FA() {
    return this.http.post<any>(
      this.setup2FAUrl,
      {},
      { headers: this.getChallengeHeaders() }
    );
  }

  get2FAQr() {
  return this.http.get(this.get2FAQrUrl, {
    headers: this.getChallengeHeaders(),
    responseType: 'blob'
  });

  }

  verify2FASetup(model: any) {
    return this.http.post<any>(
      this.verify2FASetupUrl,
      model,
      { headers: this.getChallengeHeaders() }
    );
    
  }
  
verifyLoginOtp(model: any) {
  return this.http.post<any>(
    "https://localhost:7277/api/auth/verify-login-otp",
    model,
    { headers: this.getChallengeHeaders() }
  );
}

}
