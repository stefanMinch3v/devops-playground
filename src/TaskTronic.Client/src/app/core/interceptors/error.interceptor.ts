import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpInterceptor, HttpEvent } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class ErrorInterceptorService  implements HttpInterceptor {
  constructor(
    public router: Router, 
    private notificationService: NotificationService) {
  }
 
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      retry(1),
      catchError((err) => {
        let errorDescription = '';
        const errors = err.error.errors ? err.error.errors : err.error;

        if (errors) {
          for (let key in errors) {
            let obj = errors[key];

            for (let prop in obj) {
              errorDescription += obj[prop];
            }
          }
        } else {
          errorDescription = "Something went wrong.";
        }
        
        let message = ""

        if (err.status === 401) {
          message = "401 " + errorDescription
        }
        else if (err.status === 404) {
          message = "404 " + errorDescription
        }
        else if (err.status === 400) {
          message = "400 " + errorDescription
        }
        else {
          message = "Unexpected error"
        }
        
        this.notificationService.errorMessage(message)
        return throwError(err)
      })
    )
  }
}