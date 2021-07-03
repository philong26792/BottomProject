import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartsModule } from 'ng2-charts';

import { PaginationModule } from 'ngx-bootstrap/pagination';
import { NgSelectModule } from '@ng-select/ng-select';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { MainComponent } from './main/main.component';
import { ByPoRoutingModule } from './by-po-routing.module';
import { DetailComponent } from './detail/detail.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgSelect2Module } from 'ng-select2';
import { DetailByReceivingTypeComponent } from './detail-by-receiving-type/detail-by-receiving-type.component';


@NgModule({
  imports: [
    CommonModule,
    NgSelect2Module,
    FormsModule,
    ByPoRoutingModule,
    PaginationModule,
    NgSelectModule,
    BsDatepickerModule,
    ChartsModule,
    NgxSpinnerModule
  ],
  declarations: [
    MainComponent,
    DetailComponent,
    DetailByReceivingTypeComponent
  ],
  schemas: [
    NO_ERRORS_SCHEMA
]
})
export class ByPoModule {}