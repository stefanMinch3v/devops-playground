import { FileModel } from './file.model';

export interface Folder {
    folderId: number;
    catalogId: number;
    parentId?: number;
    rootId?: number;
    name: string;
    isPrivate: boolean;
    createdBy: number;
    files: FileModel[];
    subFolders: Folder[];
    rootFolder: Folder;
    parentFolder: Folder;
    fileCount: number;
    folderCount: number;
    createDate: Date;
    creatorUsername: string;
}
