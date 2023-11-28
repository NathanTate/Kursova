import { Component, OnInit } from '@angular/core';
import { Member } from '../_modules/member';
import { MembersService } from '../_services/members.service.service';

@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit{
  members: Member[] = [];
  constructor(private memberService: MembersService) {}
  
  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers().subscribe({
      next: response => {
        this.members = response
      }
    })
  }

  onUserDeletedUpdated() {
    this.loadMembers();
  }


}
