import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit{
  loginIsVisible: boolean = false;
  visible: boolean = true;
  changeType: boolean = true;
  resetForm: FormGroup = new FormGroup({});
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, 
    private fb: FormBuilder, private toastr: ToastrService) {}

  ngOnInit() : void {
    this.initializeForm();
  }

  initializeForm() {
    this.resetForm = this.fb.group({
      resetToken: ['', Validators.required],
      password: ['', [Validators.required, 
        Validators.minLength(6), Validators.maxLength(16)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    this.resetForm.controls['password'].valueChanges.subscribe({
      next: () => this.resetForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  resetPassword() {
    this.accountService.resetPassword(this.resetForm.value).subscribe({
      next: () => {
        this.resetForm.reset()
        this.toastr.success("Password successfully changed")
        this.loginIsVisible = true
      }
    })
  }

  viewPassword() {
    this.visible = !this.visible;
    this.changeType = !this.changeType;
  }
}
