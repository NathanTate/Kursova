import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './information/about/about.component';
import { ContactComponent } from './information/contact/contact.component';
import { EmailVerifyComponent } from './email-verify/email-verify.component';
import { LoginComponent } from './login/login.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { authGuard } from './_guards/auth.guard';
import { ManageUsersComponent } from './manage-users/manage-users.component';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'forgot-password', component: ForgotPasswordComponent},
  {path: 'verify-email', component: EmailVerifyComponent},
  {path: 'login', component: LoginComponent},
  {path: 'forgot-password/reset', component: ResetPasswordComponent},
  {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'about', component: AboutComponent},
      {path: 'contact', component: ContactComponent},
      {path: 'manage-users',
      canActivate: [authGuard],
      data: {roles: ['admin', 'teacher']}, 
       component: ManageUsersComponent}
    ]
  },
  {path: '**', component: HomeComponent, pathMatch: 'full'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
