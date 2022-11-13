import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'overflowEllipsis'})
export class OverflowEllipsisPipe implements PipeTransform {
  transform(value: string, length: number = 18): string {
    if (value.length <= length) {
        return value;
    }

    let countCapitalChar = 0;
    let countLowerChar = 0;
    let countWWW = 0;

    for (const element of value) {
        if (element.toUpperCase() === 'W') {
            countWWW++;
            continue;
        }

        if (element === element.toUpperCase()) {
            countCapitalChar++;
        } else {
            countLowerChar++;
        }
    }

    if (countCapitalChar > countLowerChar) {
        length = 10;
    } else if (countCapitalChar === countLowerChar) {
        length = 14;
    }

    if (countWWW >= 10) {
        length = 10;
    }

    return value.substring(0, length) + '...';
  }
}