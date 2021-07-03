import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GenerateCollectionComponent } from './generate-collection/generate-collection.component';
import { PrintComponent } from './print/print.component';
import { TransferFormRoutingModule } from './transfer-from-routing.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FormsModule } from '@angular/forms';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PrintFormComponent } from './print-form/print-form.component';
import { NgxPrintModule } from 'ngx-print';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelect2Module } from 'ng-select2';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { CustomPipeModule } from '../../_core/_pipe/custom-pipe.module';

@NgModule({
  imports: [
    CommonModule,
    TransferFormRoutingModule,
    NgxSpinnerModule,
    FormsModule,
    BsDatepickerModule,
    NgxPrintModule,
    PaginationModule,
    NgSelect2Module,
    NgxQRCodeModule,
    CustomPipeModule
  ],
  declarations: [
    GenerateCollectionComponent,
    PrintComponent,
    PrintFormComponent,
    PrintFormComponent
  ]
})
export class TransferFormModule { }
