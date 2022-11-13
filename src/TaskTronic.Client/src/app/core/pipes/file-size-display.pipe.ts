import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'displayFileSize'})
export class FileSizeDisplayPipe implements PipeTransform {
  transform(value: number, decimals: number = 1): string {
    if (value === 0) {
        return '0 Bytes';
    }

    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;

    const i = Math.floor(Math.log(value) / Math.log(k));

    return parseFloat((value / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
