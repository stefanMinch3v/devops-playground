import { Pipe, PipeTransform } from '@angular/core';

const defaultColor = '#919AA1';
const faIconsStyles = {
 '.acc': defaultColor,
 '.flac': defaultColor,
 '.m4a': defaultColor,
 '.mp3': defaultColor,
 '.wav': defaultColor,

 '.avi': defaultColor,
 '.flv': defaultColor,
 '.mp4': defaultColor,
 '.mpg': defaultColor,
 '.mpeg': defaultColor,
 '.wmv': defaultColor,

 '.zip': defaultColor,
 '.iso': defaultColor,
 '.tar': defaultColor,
 '.rar': defaultColor,

 '.bmp': defaultColor,
 '.gif': defaultColor,
 '.jpg': defaultColor,
 '.jpeg': defaultColor,
 '.png': defaultColor,
 '.tif': defaultColor,
 '.tiff': defaultColor,

 '.doc': 'rgb(48,101,182)',
 '.docx': 'rgb(48,101,182)',

 '.xls': 'rgb(41,151,92)',
 '.xlsx': 'rgb(41,151,92)',

 '.ppt': 'rgb(242,81,40)',
 '.pptx': 'rgb(242,81,40)',

 '.msg': defaultColor,

 '.pdf': 'rgb(255,51,0)',

 '.txt': defaultColor,
 '.rtf': defaultColor
};

@Pipe({name: 'faIconColor'})
export class FaIconColorPipe implements PipeTransform {
  transform(value: string): string {
    if (!faIconsStyles[value]) {
        return defaultColor;
    }

    return faIconsStyles[value];
  }
}
