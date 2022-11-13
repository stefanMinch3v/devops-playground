import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { LoginFormModel } from '../../../core/models/login.model';
import { Router, ActivatedRoute } from '@angular/router';
import { IdentityService } from 'src/app/core/services/identity.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { notificationMessages } from 'src/app/core/notification-messages.constants';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup<LoginFormModel>;
  returnUrl: string;
  @Output() emitter: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private fb: FormBuilder, 
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private identityService: IdentityService,
    private authService: AuthService) { 
      if (this.authService.isUserAuthenticated()) {
        this.router.navigate(['/drive'])
      }
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.loginForm = this.fb.group<LoginFormModel>({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  login() {
    this.identityService.login(this.loginForm.value)
      .subscribe(res => {
        this.authService.authenticateUser(res.token);
        this.authService.saveUser(this.loginForm.value.username);
        this.authService.saveRoles(res.roles);
        this.authService.saveExpirationTime(res.expiration);

        this.notificationService.successMessage(notificationMessages.successLogin);
        this.router.navigateByUrl(this.returnUrl);
      });
  }
}
