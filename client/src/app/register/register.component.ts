import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit{
  @Output() enableLogin = new EventEmitter();
  visible: boolean = true;
  changeType: boolean = true;
  registerForm: FormGroup = new FormGroup({});
  validationErrors: string[] | undefined;


  constructor(private accountService: AccountService, 
    private fb: FormBuilder, private toastr: ToastrService) {}
  
  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, 
        Validators.minLength(6), Validators.maxLength(16)]]
    })
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => {
        this.registerForm.reset(),
        this.enableLogin.emit(true),
        this.toastr.success('Check your email for verification')
      }
    })
  }

  login() {
    this.enableLogin.emit(true);
  }

  viewPassword() {
    this.visible = !this.visible;
    this.changeType = !this.changeType;
  }

}
