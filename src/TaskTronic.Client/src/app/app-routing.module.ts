import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
    path: 'identity',
    loadChildren: () => import('./components/identity/identity-routing.module').then(m => m.IdentityRoutingModule)
  },
  {
    path: 'drive',
    loadChildren: () => import('./components/drive/drive-routing.module').then(m => m.DriveRoutingModule)
  },
  {
      path: '**',
      redirectTo: ''
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
