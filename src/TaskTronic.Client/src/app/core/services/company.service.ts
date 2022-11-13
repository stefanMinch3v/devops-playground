import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { CompanyWrapper } from '../models/company-wrapper.model';

@Injectable({
    providedIn: 'root'
})
export class CompanyService {
    private readonly DRIVE_COMPANIES = '/Companies/';
    constructor(private http: HttpClient) { }

    public getCompanies(): Observable<CompanyWrapper> {
        const url = environment.driveUrl + `${this.DRIVE_COMPANIES}GetCompanyDepartments`;
        return this.http.get(url)
            .pipe(map((response: CompanyWrapper) => response));
    }
}
