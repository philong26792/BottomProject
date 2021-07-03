import { NgModule } from "@angular/core";
import { MinusPiPe } from "./minus.pipe";
import { SumPipe } from "./sum.pipe";
import { SumAttrPipe } from "./sumAttr.pipe";


@NgModule({
    declarations: [
      SumPipe,
      SumAttrPipe,
      MinusPiPe
    ],
    exports: [SumPipe, MinusPiPe, SumAttrPipe],
  })
  export class CustomPipeModule {}