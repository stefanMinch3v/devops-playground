import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { notificationMessages } from "../notification-messages.constants";

@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    constructor(private toastr: ToastrService) { }

    successMessage = (msg) => 
        this.toastr.success(msg, notificationMessages.successTitle);

    errorMessage = (msg) => 
        this.toastr.error(msg, notificationMessages.errorTitle);

    warningMessage = (msg) => 
        this.toastr.warning(msg, notificationMessages.warningTitle);
        
    infoMessage = (msg) => 
        this.toastr.info(msg, notificationMessages.infoTitle);
}