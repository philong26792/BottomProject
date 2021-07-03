import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReceiveEnoughRoutingModule } from './receive-enough-routing.module';
import { ReceiveMainComponent } from './receive-main/receive-main.component';
import { RecordComponent } from './record/record.component';
import { RecordDetailComponent } from './record-detail/record-detail.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelect2Module } from 'ng-select2';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AlertModule } from 'ngx-bootstrap/alert';

@NgModule({
  declarations: [
    ReceiveMainComponent,
    RecordComponent,
    RecordDetailComponent
  ],
  imports: [
    CommonModule,
    ReceiveEnoughRoutingModule,
    FormsModule,
    NgSelect2Module,
    ReactiveFormsModule,
    NgxSpinnerModule,
    PaginationModule,
    NgSelect2Module,
    BsDatepickerModule,
    AlertModule.forRoot(),
  ],
  schemas: [
    NO_ERRORS_SCHEMA
]
})
export class ReceiveEnoughModule { }
