export interface FileModel {
    fileId: number;
    catalogId: number;
    folderId: number;
    blobId: number;
    fileSize: number;
    fileName: string;
    fileType: string;
    contentType: string;
    createDate: Date;
    updateDate: Date;
    creatorUsername: string;
    searchFolderNamesPath: string[];
    employeeId: number;
}
