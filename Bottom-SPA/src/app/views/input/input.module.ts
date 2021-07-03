import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { InputRoutingModule } from './input-routing.module';
import { NgSelect2Module } from 'ng-select2';
//Component
import { InputMainComponent } from './input-main/input-main.component';
import { InputPrintComponent } from './input-print/input-print.component';
import { MissingPrintComponent } from './missing-print/missing-print.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { NgxPrintModule } from 'ngx-print';
import { QrcodeAgainComponent } from './qrcode-again/qrcode-again.component';
import { AlertModule } from 'ngx-bootstrap/alert';
import { MissingAgainComponent } from './missing-again/missing-again.component';
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxSpinnerModule,
        InputRoutingModule,
        PaginationModule,
        NgSelectModule,
        NgSelect2Module,
        BsDatepickerModule,
        NgxQRCodeModule,
        NgxPrintModule,
        AlertModule.forRoot()
    ],
    declarations: [
        InputMainComponent,
        InputPrintComponent,
        MissingPrintComponent,
        QrcodeAgainComponent,
        MissingAgainComponent
    ],
    schemas: [
        NO_ERRORS_SCHEMA
    ]
})


export class InputModule {
}
