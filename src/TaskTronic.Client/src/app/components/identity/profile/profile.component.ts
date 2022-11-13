import { Component, OnInit } from '@angular/core';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { CompanyWrapper } from 'src/app/core/models/company-wrapper.model';
import { SelectedCompanyModel } from 'src/app/core/models/selected-company.model';
import { CompanyService } from 'src/app/core/services/company.service';
import { TotalView } from 'src/app/core/models/total-view.model';
import { GatewayService } from 'src/app/core/services/gateway.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  companyWrapper: CompanyWrapper;
  totalViews: Array<TotalView>;

  isLoadingCompanies: boolean;
  isLoadingViews: boolean;

  constructor(
    private readonly employeeService: EmployeeService,
    private readonly notificationService: NotificationService,
    private readonly companyService: CompanyService,
    private readonly gatewayService: GatewayService) { }

  ngOnInit() {
    this.reloadCompanies();
    this.reloadTotalViews();
  }

  public selectCompanyDepartment(companyId: number, departmentId: number): void {
    if (companyId < 1 || departmentId < 1) {
      this.notificationService.errorMessage('Invalid company/department.');
    }

    this.employeeService.setCompany(companyId, departmentId)
      .subscribe(_ => {
        this.notificationService.successMessage('Data saved.');
        this.reloadCompanies();
      });
  }

  public selectedDataMatch(companyId: number, departmentId: number, selectedData: SelectedCompanyModel): boolean {
    if (!selectedData) {
      return false;
    }

    if (selectedData.companyId === companyId && selectedData.departmentId === departmentId) {
      return true;
    }

    return false;
  }

  private reloadCompanies(): void {
    this.isLoadingCompanies = true;

    this.companyService.getCompanies()
      .subscribe(companyWrapper => {
        this.companyWrapper = companyWrapper;
        this.isLoadingCompanies = false;
      });
  }

  private reloadTotalViews(): void {
    this.isLoadingViews = true;

    this.gatewayService.getTotalViews()
      .subscribe(views => {
        this.totalViews = views;
        this.isLoadingViews = false;
      });
  }
}
