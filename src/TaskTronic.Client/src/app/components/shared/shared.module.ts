import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
    declarations: [HeaderComponent, FooterComponent],
    imports: [CommonModule, RouterModule, FormsModule, BrowserModule],
    exports: [HeaderComponent, FooterComponent]
})
export class SharedModule { }