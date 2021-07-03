import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StockAdjRoutingModule } from './stock-adj-routing.module';
import { ModifyQrcodeMainComponent } from './modify-qrcode-main/modify-qrcode-main.component';
import { NoByBatchOutComponent } from './no-by-batch-out/no-by-batch-out.component';
import { NoByBatchInComponent } from './no-by-batch-in/no-by-batch-in.component';
import { ByBatchInComponent } from './by-batch-in/by-batch-in.component';
import { ByBatchOutComponent } from './by-batch-out/by-batch-out.component';
import { ListQrcodeChangeComponent } from './list-qrcode-change/list-qrcode-change.component';
import { DetailQrcodeComponent } from './detail-qrcode/detail-qrcode.component';
import { PrintListMissingComponent } from './print-list-missing/print-list-missing.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelect2Module } from 'ng-select2';
import { HttpClientModule } from '@angular/common/http';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AlertModule } from 'ngx-bootstrap/alert';
import { NgxPrintModule } from 'ngx-print';
import { CustomPipeModule } from '../../_core/_pipe/custom-pipe.module';
import { NgSelectModule } from '@ng-select/ng-select';


@NgModule({
  declarations: [
    ModifyQrcodeMainComponent,
    NoByBatchOutComponent,
    NoByBatchInComponent,
    ByBatchInComponent,
    ByBatchOutComponent,
    ListQrcodeChangeComponent,
    DetailQrcodeComponent,
    PrintListMissingComponent
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
    BsDatepickerModule,
    AlertModule.forRoot(),
    StockAdjRoutingModule,
    NgxPrintModule,
    CustomPipeModule
  ],
  schemas: [NO_ERRORS_SCHEMA],
})
export class StockAdjModule { }
