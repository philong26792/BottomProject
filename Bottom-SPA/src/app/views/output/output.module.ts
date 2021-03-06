import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { OutputRoutingModule } from './output-routing.module';

// Component
import { OutputMainComponent } from './output-main/output-main.component';
import { OutputDetailComponent } from './output-detail/output-detail.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { NgxPrintModule } from 'ngx-print';
import { OutputProcessComponent } from './output-process/output-process.component';
import { OutputPrintQrcodeAgainComponent } from './output-print-qrcode-again/output-print-qrcode-again.component';
import { OutputDetailPreviewComponent } from './output-detail-preview/output-detail-preview.component';
import { DirectiveModule } from '../../_core/_directive/directive.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgxSpinnerModule,
        OutputRoutingModule,
        PaginationModule,
        NgSelectModule,
        BsDatepickerModule,
        NgxQRCodeModule,
        NgxPrintModule,
        DirectiveModule
    ],
    declarations: [
        OutputMainComponent,
        OutputDetailComponent,
        OutputProcessComponent,
        OutputPrintQrcodeAgainComponent,
        OutputDetailPreviewComponent
    ]
})

export class OutputModule {
}
