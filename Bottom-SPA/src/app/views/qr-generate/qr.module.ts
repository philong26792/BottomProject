import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { QrRoutingModule } from './qr-routing.module';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { AlertModule } from 'ngx-bootstrap/alert';
// Component
import { QrMainComponent } from './qr-main/qr-main.component';
import { QrPrintComponent } from './qr-print/qr-print.component';
import { QrBodyComponent } from './qr-body/qr-body.component';
import { NgxPrintModule } from 'ngx-print';
import { IntegrationInputComponent } from './integration-input/integration-input.component';
import { NgSelect2Module } from 'ng-select2';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NgSelectModule,
        NgSelect2Module,
        ReactiveFormsModule,
        NgxSpinnerModule,
        QrRoutingModule,
        PaginationModule,
        BsDatepickerModule,
        NgxQRCodeModule,
        NgxPrintModule,
        AlertModule.forRoot()
    ],
    declarations: [
        QrMainComponent,
        QrPrintComponent,
        QrBodyComponent,
        IntegrationInputComponent,
    ],
    schemas: [
        NO_ERRORS_SCHEMA
    ]
})


export class QrModule {
}
