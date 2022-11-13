import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DriveComponent } from './drive.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';

const routes: Routes = [
  {
    path: ':companyDepartmentsId',
    component: DriveComponent,
    canActivate: [AuthGuard],
    data: { kind: 'root' }
  },
  {
    path: ':companyDepartmentsId/:selectedFolderId',
    component: DriveComponent,
    canActivate: [AuthGuard],
    data: { kind: 'folder' }
  },
  {
    path: '',
    component: DriveComponent,
    canActivate: [AuthGuard],
    data: { kind: 'root' }
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DriveRoutingModule { }
