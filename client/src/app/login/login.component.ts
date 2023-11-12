import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{
  @Output() cancelLogin = new EventEmitter();
  model: any = {};

  constructor(private accountService: AccountService, private router: Router) {}

  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => this.router.navigateByUrl('/about')
    })
  }

  register() {
    this.cancelLogin.emit(false);
  }
  
}
