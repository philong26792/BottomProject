import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'sum'
})
export class SumPipe implements PipeTransform {
  transform(items: number[]): any {
    return items.reduce((a,c) => {return a + c});
  }
}