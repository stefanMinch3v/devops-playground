import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { NotificationService } from './notification.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection;

    constructor(
        private readonly notificationService: NotificationService,
        private readonly authService: AuthService) { }

    public subscribe = () => {
        const options = {
            accessTokenFactory: () => {
                return this.authService.getToken();
            }
        };

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:5008/notifications', options)
            .build();

        this.hubConnection
            .start()
            .then(() => console.log('Connection started'))
            .catch(err => console.log('Error while starting connection: ' + err));

        this.hubConnection.on('ReceiveNotification', (data) => {
            console.log(data);
            this.notificationService.successMessage(`Success uploaded file/folder SignalR. TODO: distinguish`);
        });
    }
}
