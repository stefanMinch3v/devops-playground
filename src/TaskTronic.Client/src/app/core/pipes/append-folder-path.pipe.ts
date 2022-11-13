import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'appendFolderPath'})
export class AppendFolderPath implements PipeTransform {
  private readonly SYMBOL = ' > ';

  transform(value: string, searchFolderNamesPath: string[]): string {
    return searchFolderNamesPath
        .join(this.SYMBOL) + `${this.SYMBOL}${value}`;
  }
}
