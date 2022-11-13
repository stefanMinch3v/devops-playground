import { Component, OnInit } from '@angular/core';
import { IdentityService } from 'src/app/core/services/identity.service';
import { AuthService } from 'src/app/core/services/auth.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { notificationMessages } from 'src/app/core/notification-messages.constants';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(
    private identityService: IdentityService,
    public authService: AuthService,
    private notificationService: NotificationService) { }

  ngOnInit(): void {
  }

  public logout(): void {
      this.identityService.logout();
      this.notificationService.successMessage(notificationMessages.successLogout);
  }

  public isUserAuth(): boolean {
      return this.authService.isUserAuthenticated();
  }
}
