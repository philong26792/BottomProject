import { NgModule, NO_ERRORS_SCHEMA } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelect2Module } from 'ng-select2';
import { HttpClientModule } from '@angular/common/http';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AlertModule } from 'ngx-bootstrap/alert';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { NgxPrintModule } from "ngx-print";
import { ModalModule } from 'ngx-bootstrap/modal';

import { MergeQrcodeRoutingModule } from './merge-qrcode-routing.module';
import { MergeQrcodeMainComponent } from './merge-qrcode-main/merge-qrcode-main.component';
import { QrcodeAfterMergeComponent } from './qrcode-after-merge/qrcode-after-merge.component';
import { QrcodeDetailComponent } from './qrcode-detail/qrcode-detail.component';
import { SplitMainComponent } from './split-main/split-main.component';
import { SplitProcessComponent } from './split-process/split-process.component';
import { SplitDetailComponent } from './split-detail/split-detail.component';
import { CustomPipeModule } from "../../_core/_pipe/custom-pipe.module";
import { SplitQrcodeDetailComponent } from './split-qrcode-detail/split-qrcode-detail.component';
import { OtherSplitMainComponent } from './other-split-main/other-split-main.component';
import { OtherSplitDetailComponent } from './other-split-detail/other-split-detail.component';
import { OtherSplitProcessComponent } from './other-split-process/other-split-process.component';
import { OtherSplitQrcodeDetailComponent } from './other-split-qrcode-detail/other-split-qrcode-detail.component';
import { DirectiveModule } from "../../_core/_directive/directive.module";
import { OtherSplitQrcodeDetailChildComponent } from './other-split-qrcode-detail-child/other-split-qrcode-detail-child.component';
import { SplitQrcodeDetailChildComponent } from './split-qrcode-detail-child/split-qrcode-detail-child.component';


@NgModule({
  declarations: [
      MergeQrcodeMainComponent,
      QrcodeAfterMergeComponent,
      QrcodeDetailComponent,
      SplitMainComponent,
      SplitProcessComponent,
      SplitDetailComponent,
      SplitQrcodeDetailComponent,
      SplitQrcodeDetailChildComponent,
      OtherSplitMainComponent,
      OtherSplitDetailComponent,
      OtherSplitProcessComponent,
      OtherSplitQrcodeDetailComponent,
      OtherSplitQrcodeDetailChildComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelect2Module,
    PaginationModule,
    HttpClientModule,
    NgSelectModule,
    NgxSpinnerModule,
    TooltipModule.forRoot(),
    BsDatepickerModule.forRoot(),
    AlertModule.forRoot(),
    NgxQRCodeModule,
    NgxPrintModule,
    CustomPipeModule,
    MergeQrcodeRoutingModule,
    DirectiveModule,
    ModalModule.forRoot()
  ],
  schemas: [NO_ERRORS_SCHEMA],
})
export class MergeQrcodeModule { }
