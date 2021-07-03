import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ReceiveMaterialComponent } from './receive-material/receive-material.component';
import { ReportRoutingModule } from './report-routing.module';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AlertModule } from 'ngx-bootstrap/alert';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelect2Module } from 'ng-select2';
import { CompareReportComponent } from './compare-report/compare-report.component';


@NgModule({
    imports: [
        CommonModule,
        NgSelect2Module,
        FormsModule,
        ReactiveFormsModule,
        ReportRoutingModule,
        NgxSpinnerModule,
        BsDatepickerModule,
        PaginationModule,
        AlertModule.forRoot()
    ],
    declarations: [
        ReceiveMaterialComponent,
        CompareReportComponent,
    ],
    schemas: [
        NO_ERRORS_SCHEMA
    ]
})

export class ReportModule {
}
