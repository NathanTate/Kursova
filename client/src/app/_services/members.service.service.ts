import { Injectable } from '@angular/core';
import { Member } from '../_modules/member';
import { HttpClient } from '@angular/common/http';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = 'https://localhost:5001/api/'
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getMembers() {

    return this.http.get<Member[]>(this.baseUrl + "users");
  }

  deleteUser(email: String) {
    return this.http.delete(this.baseUrl + "users/delete/" + email)
  }

  updateRole(model: any) {
    return this.http.put(this.baseUrl + "users", model);
  }
}
