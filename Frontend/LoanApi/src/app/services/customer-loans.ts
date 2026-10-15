
import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class customer_loans{
  http=inject(HttpClient);

  // on Customer forclosure request page 
  //for getting all the loan accounts of a respective customer
  url="https://localhost:7277/api/customer/foreclosure/loans";
  getLoanAccounts(){
    return this.http.get<any>(this.url);
  }

  url2="https://localhost:7277/api/customer/foreclosure/summary";
  getCardsData(loanAccountId: number) {
return this.http.get<any>(`${this.url2}/${loanAccountId}`);
}

url3="https://localhost:7277/api/customer/foreclosure";
AddForeClosure(data:any){
  return this.http.post<any>(this.url3,data);
}


url4="https://localhost:7277/api/customer/foreclosure/requests";
getForeClosureRequests(){
  return this.http.get<any>(this.url4);
}

url5="https://localhost:7277/api/customer/foreclosure/payment/create-order";
createPaymentOrder(requestId:number){
  return this.http.post<any>(`${this.url5}/${requestId}`,{});
}


url6="https://localhost:7277/api/customer/foreclosure/payment";
verifyPayment(
  data: {
    requestId: number;
    razorpayOrderId: string;
    razorpayPaymentId: string;
    razorpaySignature: string;
  }) {
    return this.http.post<any>(
      `${this.url6}/verify`,
      data
    );
  }

  // for payment history page
url7 = "https://localhost:7277/api/customer/payment-history";

getPaymentHistory(loanAccountId: number | null, months: number) {
  let url8 = `${this.url7}?months=${months}`;

  if (loanAccountId !== null) {
    url8 += `&loanAccountId=${loanAccountId}`;
  }

  return this.http.get<any>(url8);
}


//for officer page 
  url8 = "https://localhost:7277/api/officer/foreclosure";
getAllForeClosureRequests(){
  return this.http.get<any>(this.url8);
}

getForeClosureRequestById(requestId:number){
  return this.http.get<any>(`${this.url8}/${requestId}`);
}

changeForeClosureStatus(requestId: number, status: string) {
  return this.http.put<any>(
    `${this.url8}/${requestId}`,
    { status: status }
  );
}
}
