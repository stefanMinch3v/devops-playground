import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild, ElementRef, TemplateRef } from '@angular/core';
import { Location } from '@angular/common';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DriveService } from 'src/app/core/services/drive.service';
import { NotificationService } from 'src/app/core/services/notification.service';
import { AuthService } from 'src/app/core/services/auth.service';
import * as plupload from 'plupload';
import { Folder } from '../../core/models/folder.model';
import { FileModel } from '../../core/models/file.model';
import { FolderIdName } from '../../core/models/folder-id-name.model';
import { SignalRService } from 'src/app/core/services/signalR.service';
import { FaIconPipe } from 'src/app/core/pipes/fa-icons.pipe';
import { EmployeeService } from 'src/app/core/services/employee.service';
import { CommonHelper } from 'src/app/core/helpers/common.helper';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-drive',
  templateUrl: './drive.component.html',
  styleUrls: ['./drive.component.scss'],
  providers: [FaIconPipe]
})
export class DriveComponent implements OnInit {
  private readonly FILE_MAX_SIZE = 2147483647;
  // PLUPLOAD
  public uploader: plupload.Uploader;
  private pluploadFilesPercentage: HTMLElement;
  //

  public companyDepartmentsId: number;
  public selectedFolderId: number;
  public folder: Folder;
  public newFolderName: string;
  public selectedFolder: Folder;
  public newFileName: string;
  private parentArray: FolderIdName[] = [];
  public parentFolderChain: FolderIdName[] = [];
  public isLoading = true;

  // search
  @ViewChild('searchValue', {static: true}) searchValue: ElementRef;
  public hasSearchResult: boolean;
  public searchResults: FileModel[] = [];
  //

  // bootstrap modal
  private modalElementId: number;
  private modalCurrentFolderId: number;
  modalNameToChange: string;
  modalIsFolder: boolean;
  modalIsCreate: boolean;
  modalIsWordFile: boolean;
  modalRef: BsModalRef;
  @ViewChild('modalTemplate', { static: true }) modalTemplateRef: TemplateRef<any>;

  public constructor(
    private readonly driveService: DriveService,
    private readonly notificationService: NotificationService,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly location: Location,
    private readonly signalRService: SignalRService,
    private readonly faIconPipe: FaIconPipe,
    private readonly employeeService: EmployeeService,
    private readonly modalService: BsModalService) {
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  public ngOnInit(): void {
    this.employeeService.getCompanyDepartmentsSignId()
      .subscribe(id => {
        if (id < 1) {
          this.notificationService.errorMessage('Please pick company/department from your profile!');
        } else {
          this.companyDepartmentsId = id;
          this.selectedFolderId = Number(this.route.snapshot.paramMap.get('selectedFolderId'));

          this.signalRService.subscribe();

          this.route.data.subscribe(data => {
            switch (data.kind) {
              case 'root':
                this.getRootFolder();
                break;
              case 'folder':
                this.getFolder(this.selectedFolderId);
                break;
            }});
        }
      });
  }

  public navigateToRoot(): void {
    const url = this.router.createUrlTree(['drive', this.companyDepartmentsId]);
    this.location.go(url.toString());
    this.getRootFolder();
  }

  // Create/Edit for file/folder
  public openModal(
      name: string,
      isFolder: boolean,
      currentFolderId: number,
      elementId: number,
      isCreateOperation: boolean,
      isWordFile = true): void {
    this.modalNameToChange = name;
    this.modalIsFolder = isFolder;
    this.modalElementId = elementId;
    this.modalCurrentFolderId = currentFolderId;
    this.modalIsCreate = isCreateOperation;
    this.modalIsWordFile = isWordFile;

    this.modalRef = this.modalService.show(this.modalTemplateRef);
  }

  public isCurrentUserAuthor(userEmail: string): boolean {
    const emailStartIndex = userEmail.indexOf('@');
    const authorUsername = userEmail.substring(0, emailStartIndex);

    return this.authService.getUser() === authorUsername;
  }

  public getFontAwesomeIconName(fileType: string): string {
    return this.faIconPipe.transform(fileType) + ' pr-3';
  }

  // breadcrumbs actions
  private setUrl(folder: Folder): void {
    this.parentArray = [];
    const url = this.router.createUrlTree(['drive', this.companyDepartmentsId]);
    let newUrl = url.toString();

    if (folder.parentFolder) {
      this.getAllParents(folder);
    } else {
      this.parentArray.push(new FolderIdName(folder.folderId, folder.name));
    }

    this.parentArray = this.parentArray.reverse();
    this.parentFolderChain = this.parentArray;

    this.parentArray.forEach(el => {
      newUrl = newUrl.concat('/', el.id.toString());
    });

    this.location.go(newUrl);
  }

  private getAllParents(folder: Folder): void {
    this.parentArray.push(new FolderIdName(folder.folderId, folder.name));
    if (folder.parentFolder) {
      this.getAllParents(folder.parentFolder);
    }
  }

  // FOLDER actions
  public openFolder(folderId: number): void {
    this.getFolder(folderId);
  }

  public deleteFolder(folder: Folder): void {
    const confirmation = confirm('Confirm delete of ' + folder.name);

    if (confirmation) {
      this.driveService.deleteFolder(folder.catalogId, folder.folderId)
        .subscribe(res => {
          if (res) {
            this.reloadFolder();
          }
        });
    }
  }

  public createFolder(): void {
    if (!this.modalNameToChange) {
        this.notificationService.errorMessage('Empty name');
        return;
      } else if (this.modalNameToChange.length > 50) {
        this.notificationService.errorMessage('The name is too long');
        return;
      }

    this.driveService.createFolder(this.folder, this.modalNameToChange)
      .subscribe(_ => {
        this.modalRef.hide();
        this.reloadFolder();
      });
  }

  public togglePrivate(folderId: number, catalogId: number): void {
    this.driveService.togglePrivate(folderId, catalogId)
      .subscribe(_ => this.reloadFolder());
  }

  public renameFolder(): void {
    if (!this.modalNameToChange ||
        this.modalNameToChange.length < 2 ||
        this.modalNameToChange.length > 255) {
      this.notificationService.warningMessage('Name must be at least 2 and maximum 255 symbols long!');
      return;
    }

    const hasInvalidChars = CommonHelper.hasInvalidCharacters(this.modalNameToChange);
    if (hasInvalidChars) {
      this.notificationService.warningMessage('\\,/,:,*,?,<,>,|,\" are not allowed!');
      return;
    }

    this.driveService.renameFolder(this.folder.catalogId, this.modalElementId, this.modalNameToChange)
      .subscribe(_ => {
        this.modalRef.hide();
        this.reloadFolder();
      });
  }

  private getRootFolder(): void {
    this.isLoading = true;
    this.driveService.getRootFolder(this.companyDepartmentsId)
      .subscribe(folder => {
        this.folder = folder;
        this.setUrl(this.folder);
        this.isLoading = false;

        this.pluploadInit();
      }, error => {
        this.isLoading = false;
      });
  }

  private reloadFolder(): void {
    if (this.folder) {
      if (this.folder.rootId) {
        this.getFolder(this.folder.folderId);
      } else {
        this.getRootFolder();
      }
    }
  }

  private getFolder(folderId: number): void {
    this.isLoading = true;

    this.driveService.getFolder(folderId, this.companyDepartmentsId)
      .subscribe(folder => {

        this.folder = folder;
        this.setUrl(this.folder);
        this.isLoading = false;

        this.pluploadInit();
      }, error => {
        this.isLoading = false;
        this.navigateToRoot();
      });
  }

  // FILE ACTIONS
  public downloadFile(file: FileModel, shouldOpen: boolean = false): void {
    const downloadUrl = this.driveService.downloadFile(file.catalogId, file.folderId, file.fileId, shouldOpen);
    window.open(downloadUrl);
  }

  public deleteFile(file: FileModel): void {
    const confirmation = confirm('Confirm delete of ' + file.fileName);

    if (confirmation) {
      this.driveService.deleteFile(file.catalogId, file.folderId, file.fileId)
        .subscribe(res => {
          if (res) {
            this.reloadFolder();
          } else {
            this.notificationService.warningMessage('Could not remove the data');
          }
      });
    }
  }

  public searchForFiles(e): void {
    e.preventDefault();
    const val = this.searchValue.nativeElement.value;

    if (val.length > 0) {
      this.isLoading = true;
      this.driveService.searchForFile(this.folder.catalogId, this.folder.folderId, val)
        .subscribe(result => {
          this.hasSearchResult = true;
          this.searchResults = result;
          this.isLoading = false;
        });
    } else {
      this.hasSearchResult = false;
    }
  }

  public updateSearch(): void {
    const val = this.searchValue.nativeElement.value;
    if (val.length === 0) {
      this.hasSearchResult = false;
    }
  }

  public createNewDocumentFile(): void {
    if (this.modalIsWordFile) {
      this.driveService.createNewFile(this.folder.catalogId, this.folder.folderId, 1, this.modalNameToChange)
        .subscribe(_ => {
          this.modalRef.hide();
          this.reloadFolder();
        });
    } else {
      this.driveService.createNewFile(this.folder.catalogId, this.folder.folderId, 2, this.modalNameToChange)
        .subscribe(_ => {
          this.modalRef.hide();
          this.reloadFolder();
        });
    }
  }

  public renameFile(): void {
    if (!this.modalNameToChange ||
        this.modalNameToChange.length < 2 ||
        this.modalNameToChange.length > 255) {
      this.notificationService.warningMessage('Name must be at least 2 and maximum 255 symbols long!');
      return;
    }

    const hasInvalidChars = CommonHelper.hasInvalidCharacters(this.modalNameToChange);
    if (hasInvalidChars) {
      this.notificationService.warningMessage('\\,/,:,*,?,<,>,|,\" are not allowed!');
      return;
    }

    this.driveService.renameFile(this.folder.catalogId, this.folder.folderId, this.modalElementId, this.modalNameToChange)
      .subscribe(_ => {
        this.modalRef.hide();
        this.reloadFolder();
      });
  }

  // PLUPLOAD, copy-paste from docs
  pluploadInit(): void {
    if (this.uploader) {
      this.clearPluploadContainer();
      this.uploader.settings.multipart_params = this.getGroupParameters();
    } else {
      this.pluploadFilesPercentage = document.getElementById('file-list');
      this.uploader = this.initPlupload();
    }
  }

  public startUpload(): void {
    this.uploader.start();
  }

  public getGroupParameters(): object {
    if (this.folder) {
      return {
        catalogId: this.folder.catalogId.toString(),
        folderId: this.folder.folderId.toString()
      };
    }
  }

  initPlupload(): plupload.Uploader {
    const uploader = new plupload.Uploader({
        runtimes : 'html5,flash,silverlight,html4',
        browse_button : 'pick-files', // can pass in id
        container: document.getElementById('pluploadcontainer'),
        multipart_params: this.getGroupParameters(),
        chunk_size: '10mb',
        url : environment.driveUrl + '/files/UploadFileToFolder',
        headers: { Authorization: 'Bearer ' + this.authService.getToken() },
        filters : { prevent_empty: false } as any,
        // Flash settings
        flash_swf_url : '/plupload/js/Moxie.swf',
        // Silverlight settings
        silverlight_xap_url : '/plupload/js/Moxie.xap',
        init: {
            PostInit: () => {
                this.clearPluploadContainer();
                document.getElementById('upload-files').onclick = () => {
                    uploader.start();
                    return false;
                };
            },
            FilesAdded: (up, files) => {
                plupload.each(files, (file) => {
                    this.pluploadFilesPercentage
                      .innerHTML += '<div id="' + file.id + '">' + file.name + ' (' + plupload.formatSize(file.size) + ') <b></b></div>';
                });
            },
            UploadProgress: (up, file) => {
                document.getElementById(file.id).getElementsByTagName('b')[0].innerHTML = '<span>' + file.percent + '%</span>';
            },
            UploadComplete: (up, file) => {
              if (file.length > 0) {
                this.uploader.files.length = 0;
                this.reloadFolder();
              } else {
                this.notificationService.warningMessage('No files selected');
              }
            },
            Error: (up, err) => {
              console.log(err);
              this.notificationService.errorMessage(err);
            }
        }
    });

    uploader.init();

    return uploader;
  }

  private clearPluploadContainer(): void {
    this.pluploadFilesPercentage.innerHTML = '';
  }
}
