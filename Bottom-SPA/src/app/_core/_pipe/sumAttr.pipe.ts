import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'sumAttr'
})
export class SumAttrPipe implements PipeTransform {
    transform(items: object[], attr: string): any {
        return items.reduce((sum, item) => sum + item[attr], 0);
    }
}