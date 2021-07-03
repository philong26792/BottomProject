import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { NgxPrintModule } from 'ngx-print';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AlertModule } from 'ngx-bootstrap/alert';

import { TransferMainComponent } from './transfer-main/transfer-main.component';
import { TransferRoutingModule } from './transfer-routing.module';
import { TransferHistoryComponent } from './transfer-history/transfer-history.component';
import { TransferDetailComponent } from './transfer-detail/transfer-detail.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgSelect2Module } from "ng-select2";
import { CustomPipeModule } from '../../_core/_pipe/custom-pipe.module';

@NgModule({
  imports: [
    CommonModule,
    TransferRoutingModule,
    FormsModule,
    NgxQRCodeModule,
    NgxPrintModule,
    BsDatepickerModule,
    NgxSpinnerModule,
    NgSelect2Module,
    PaginationModule.forRoot(),
    AlertModule.forRoot(),
    CustomPipeModule,
    ReactiveFormsModule
  ],
  declarations: [
    TransferMainComponent,
    TransferHistoryComponent,
    TransferDetailComponent,
  ],
  schemas: [NO_ERRORS_SCHEMA],
})
export class TransferModule {}
