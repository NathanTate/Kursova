import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit{
  model: any = {};
  isTokenInputVisible = false;

  constructor(private accountService: AccountService) {}

  ngOnInit(): void {
  }

  sendEmail() {
    this.accountService.sentEmail(this.model).subscribe({
      next: () => this.isTokenInputVisible = true
    })
  }
}
