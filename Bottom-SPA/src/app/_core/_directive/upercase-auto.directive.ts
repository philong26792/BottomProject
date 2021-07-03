import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appUpercaseAuto]'
})
export class UpercaseAutoDirective {
  constructor(private host: ElementRef) {}

  /** Trata as teclas */
  @HostListener('keyup', ['$event'])
  onKeyDown() {
    this.host.nativeElement.value = this.host.nativeElement.value.toUpperCase();
  }
}
