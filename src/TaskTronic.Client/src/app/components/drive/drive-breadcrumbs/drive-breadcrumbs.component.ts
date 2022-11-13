import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FolderIdName } from '../../../core/models/folder-id-name.model';

@Component({
  // tslint:disable-next-line: component-selector
  selector: 'drive-breadcrumbs',
  templateUrl: 'drive-breadcrumbs.component.html',
  styleUrls: ['drive-breadcrumbs.component.css']
})
export class DriveBreadcrumbs {
  @Input() public parentFolderChain: FolderIdName[];
  @Output() public rootFolderClicked: EventEmitter<FolderIdName> = new EventEmitter();
  @Output() public otherFolderClicked: EventEmitter<FolderIdName> = new EventEmitter();
}
