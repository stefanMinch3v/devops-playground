import { Component, OnInit } from '@angular/core';
import { RegisterModelForm } from '../../../core/models/register.model';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IdentityService } from 'src/app/core/services/identity.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { EmployeeService } from 'src/app/core/services/employee.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup<RegisterModelForm>;

  constructor(
    private fb: FormBuilder,
     private identityService: IdentityService, 
     private router: Router,
     private authService: AuthService,
     private employeeService: EmployeeService) { }

  ngOnInit(): void {
    this.registerForm = this.fb.group<RegisterModelForm>({
      email: ['', Validators.required],
      username: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    })
  }

  register(): void {
    this.identityService.register(this.registerForm.value)
      .subscribe(res => {
        this.identityService.login({username: this.registerForm.value.username, password: this.registerForm.value.password})
          .subscribe(_ => {
            this.authService.authenticateUser(res.token);
            this.authService.saveUser(this.registerForm.value.username);
            this.authService.saveRoles(res.roles);
            this.authService.saveExpirationTime(res.expiration);
          });

        this.router.navigate(['/identity/login']);
    })
  }

}
