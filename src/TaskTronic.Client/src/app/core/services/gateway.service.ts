import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { TotalView } from '../models/total-view.model';

@Injectable({
    providedIn: 'root'
})
export class GatewayService {
    constructor(private http: HttpClient) { }

    public getTotalViews(): Observable<Array<TotalView>> {
        const url = environment.gatewayUrl + '/Drive/MyFolders';
        return this.http.get(url)
            .pipe(map((response: Array<TotalView>) => response));
    }
}
