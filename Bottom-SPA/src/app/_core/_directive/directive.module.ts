import { NgModule } from '@angular/core';
import { UpercaseAutoDirective } from './upercase-auto.directive';

@NgModule({
  declarations: [UpercaseAutoDirective],
  exports: [UpercaseAutoDirective]
})
export class DirectiveModule {}
