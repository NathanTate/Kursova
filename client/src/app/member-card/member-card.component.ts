import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Member } from '../_modules/member';
import { MembersService } from '../_services/members.service.service';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { User } from '../_modules/user';
import { take } from 'rxjs';
import { NgConfirmService } from 'ng-confirm-box';
import { updateRole } from '../_modules/updateRole';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined;
  @Output() userDeletedUpdated = new EventEmitter<void>();
  currentUser: User | null = null;
  updateRole: updateRole  = {email: '', roleId: 0};
  roles = [
    {id:0,name:'admin'},
    {id:1,name:'teacher'},
    {id:2,name:'student'}
  ];

  constructor(private memberService: MembersService, private confirmService: NgConfirmService, public accountService: AccountService, private toastr: ToastrService) 
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.currentUser = user;
      }
    })
  }
  
  ngOnInit(): void {
    if(this.member) {
      this.updateRole.email = this.member.email;
      console.log(this.member.email)
      const foundRole = this.roles.find(role => role.name === this.member?.role)
      console.log(foundRole)
      if(foundRole)
      {
        this.updateRole.roleId = foundRole.id
        console.log(this.updateRole)
      }
    }
  }

  updateUsersRole(roleId: number) {
    this.updateRole.roleId = roleId;
    this.memberService.updateRole(this.updateRole).subscribe({
      next: () => {
        this.toastr.success(`${this.member?.userName} 
        role is changed to ${this.roles[this.updateRole.roleId].name}`);
        this.userDeletedUpdated.emit();
      }
    })
  }

  deleteUser(member: Member) {
    this.confirmService.showConfirm("Are you sure to delete " + member.userName,
    () => {
      this.memberService.deleteUser(member.email).subscribe({
        next: () => {
          this.toastr.success('You have deleted ' + member.userName),
          this.userDeletedUpdated.emit();
        }
      })
    },
    () => {
      this.toastr.info("Keep it up");
    })
  }
 
}
