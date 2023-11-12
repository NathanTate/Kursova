import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-email-verify',
  templateUrl: './email-verify.component.html',
  styleUrls: ['./email-verify.component.css']
})
export class EmailVerifyComponent implements OnInit{
  isVerified = true;

  constructor(private accountService: AccountService, private route: ActivatedRoute) {}
  
  ngOnInit(): void {
    this.verifyEmail();
  }

  verifyEmail() {
    var email = this.route.snapshot.queryParams['email'];
    var token = this.route.snapshot.queryParams['token'];
    if(!(email && token)) return;
    this.accountService.verifyEmail(email, token).subscribe({
      next: () => this.isVerified = false,
      error: () => this.isVerified = true
    })
  }
}
