import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from 'src/app/core/services/auth.service';
import { DriveComponent } from './drive.component';
import { DriveService } from 'src/app/core/services/drive.service';
import { DriveBreadcrumbs } from './drive-breadcrumbs/drive-breadcrumbs.component';
import { FileSizeDisplayPipe } from 'src/app/core/pipes/file-size-display.pipe';
import { FaIconPipe } from 'src/app/core/pipes/fa-icons.pipe';
import { AppendFolderPath } from 'src/app/core/pipes/append-folder-path.pipe';
import { FaIconColorPipe } from 'src/app/core/pipes/fa-icons-color.pipe';
import { ModalModule } from 'ngx-bootstrap/modal';
import { OverflowEllipsisPipe } from 'src/app/core/pipes/overflow-ellipsis.pipe';

@NgModule({
  declarations: [
    DriveComponent,
    DriveBreadcrumbs,
    FileSizeDisplayPipe,
    OverflowEllipsisPipe,
    FaIconPipe,
    AppendFolderPath,
    FaIconColorPipe
  ],
  imports: [CommonModule, ModalModule.forRoot()],
  providers: [DriveService, AuthService],
})
export class DriveModule { }
