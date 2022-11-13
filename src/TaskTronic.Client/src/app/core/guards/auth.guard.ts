import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService, private router: Router) { }

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<boolean> | Promise<boolean> | boolean {
        return this.check(state);
    }

    check(state: RouterStateSnapshot): boolean {
        const expirationTime = new Date(this.authService.getExpirationTime());
        if (this.authService.isUserAuthenticated() && expirationTime > new Date()) {
            return true;
        }

        this.authService.deauthenticateUser();
        this.router.navigate(['/identity/login'], { queryParams: { returnUrl: state.url }});
        return false;
    }
}