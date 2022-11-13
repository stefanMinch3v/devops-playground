import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IdentityModule } from './components/identity/identity.module';
import { TokenInterceptor } from './core/interceptors/token.interceptor';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { SharedModule } from './components/shared/shared.module';
import { ToastrModule } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { CommonModule } from '@angular/common';
import { ErrorInterceptorService } from './core/interceptors/error.interceptor';
import { DriveModule } from './components/drive/drive.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    ToastrModule.forRoot(environment.toastr),
    AppRoutingModule,
    IdentityModule,
    SharedModule,
    DriveModule
  ],
  providers: [
    {
        provide: HTTP_INTERCEPTORS,
        useClass: TokenInterceptor,
        multi: true
    },
    {
        provide: HTTP_INTERCEPTORS,
        useClass: ErrorInterceptorService,
        multi: true
    },
    AuthGuard, AdminGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
