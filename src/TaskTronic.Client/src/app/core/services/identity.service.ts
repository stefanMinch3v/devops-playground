import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';
import { PayloadModel } from '../models/payload.model';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class IdentityService {
    constructor(
        private http: HttpClient,
        private authService: AuthService,
        private router: Router) { }

    public login(user): Observable<PayloadModel> {
        const url = environment.identityUrl + '/identity/login';
        return this.http.post(url, user)
            .pipe(map((response: PayloadModel) => response));
    }

    public register(user): Observable<any> {
        const url = environment.identityUrl + '/identity/register';
        return this.http.post(url, user);
    }

    public logout() {
        this.authService.deauthenticateUser();

        this.router.navigate(['/']);
    }
}
