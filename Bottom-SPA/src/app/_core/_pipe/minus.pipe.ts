import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
    name: 'minus'
})
export class MinusPiPe implements PipeTransform {
    transform(item1s: object[], item2s: object[], attr: string): any {
        let total1 = item1s.reduce((sum, item) => sum + item[attr], 0);
        let total2 = item2s.reduce((sum, item) => sum + item[attr], 0);
        return total1 - total2;
    }
}