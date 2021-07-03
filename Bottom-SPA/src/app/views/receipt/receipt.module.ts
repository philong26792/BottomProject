import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ReceiptRoutingModule } from './receipt-routing.module';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AlertModule } from 'ngx-bootstrap/alert';
import { NgSelect2Module } from 'ng-select2';
//Component
import { ReceiptMainComponent } from './receipt-main/receipt-main.component';
import { RecordComponent } from './record/record.component';
import { RecordFormComponent } from './record-form/record-form.component';
import { RecordFormBatchesComponent } from './record-form-batches/record-form-batches.component';
import { RecordDetailComponent } from './record-detail/record-detail.component';
import { RecordEditComponent } from './record-edit/record-edit.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NgSelect2Module,
        ReactiveFormsModule,
        NgxSpinnerModule,
        ReceiptRoutingModule,
        PaginationModule,
        NgSelectModule,
        BsDatepickerModule,
        AlertModule.forRoot()
    ],
    declarations: [
        ReceiptMainComponent,
        RecordComponent,
        RecordFormComponent,
        RecordFormBatchesComponent,
        RecordDetailComponent,
        RecordEditComponent
    ],
    schemas: [
        NO_ERRORS_SCHEMA
    ]
})

export class ReceiptModule {
}
