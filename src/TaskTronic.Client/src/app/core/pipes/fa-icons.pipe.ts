import { Pipe, PipeTransform } from '@angular/core';

const fileExtensionFormats = {
    '.acc': 'fas fa-file-audio',
    '.flac': 'fas fa-file-audio',
    '.m4a': 'fas fa-file-audio',
    '.mp3': 'fas fa-file-audio',
    '.wav': 'fas fa-file-audio',

    '.avi': 'fas fa-file-video',
    '.flv': 'fas fa-file-video',
    '.mp4': 'fas fa-file-video',
    '.mpg': 'fas fa-file-video',
    '.mpeg': 'fas fa-file-video',
    '.wmv': 'fas fa-file-video',

    '.zip': 'fas fa-file-archive',
    '.iso': 'fas fa-file-archive',
    '.tar': 'fas fa-file-archive',
    '.rar': 'fas fa-file-archive',

    '.bmp': 'fas fa-file-image',
    '.gif': 'fas fa-file-image',
    '.jpg': 'fas fa-file-image',
    '.jpeg': 'fas fa-file-image',
    '.png': 'fas fa-file-image',
    '.tif': 'fas fa-file-image',
    '.tiff': 'fas fa-file-image',

    '.doc': 'fas fa-file-word',
    '.docx': 'fas fa-file-word',

    '.xls': 'fas fa-file-excel',
    '.xlsx': 'fas fa-file-excel',

    '.ppt': 'fas fa-file-powerpoint',
    '.pptx': 'fas fa-file-powerpoint',

    '.msg': 'fas fa-envelope',

    '.pdf': 'fas fa-file-pdf',

    '.txt': 'fas fa-file-alt',
    '.rtf': 'fas fa-file-alt'
};
// all unknown: File

@Pipe({name: 'faIcon'})
export class FaIconPipe implements PipeTransform {
  transform(value: string): string {
    if (!fileExtensionFormats[value]) {
        return 'fas fa-file';
    }

    return fileExtensionFormats[value];
  }
}
