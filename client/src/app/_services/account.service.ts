import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map } from 'rxjs';
import { User } from '../_modules/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/'
  private currentUserSourse = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSourse.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe (
      map((response: User) => {
        const user = response;
        if(user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + "account/register", model);
  }

  verifyEmail(email: string, token: string) {
    let url = this.baseUrl + `account/verify-email?email=${email}&token=${token}`
    return this.http.post(url, {});
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user))
    this.currentUserSourse.next(user);
  }
  
  logout() {
    localStorage.removeItem('user');
    this.currentUserSourse.next(null);
  }

  sentEmail(model: any) {
    return this.http.post(this.baseUrl + 'account/forgot-password', model);
  }

  resetPassword(model: any) {
    return this.http.put(this.baseUrl + 'account/reset-password', model);
  }
}
