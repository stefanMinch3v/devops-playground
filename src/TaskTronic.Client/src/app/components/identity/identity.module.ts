import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { IdentityRoutingModule } from './identity-routing.module';
import { RegisterComponent } from './register/register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxStronglyTypedFormsModule } from 'ngx-strongly-typed-forms';
import { IdentityService } from 'src/app/core/services/identity.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { HttpClientModule } from '@angular/common/http';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { BrowserModule } from '@angular/platform-browser';
import { ProfileComponent } from './profile/profile.component';


@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ProfileComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    IdentityRoutingModule,
    FormsModule,
    NgxStronglyTypedFormsModule,
    ReactiveFormsModule
  ],
  providers: [IdentityService, AuthService, EmployeeService]
})
export class IdentityModule { }
