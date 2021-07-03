import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MainComponent } from './main/main.component';
import { ByRackRoutingModule } from './by-rack-routing.module';
import { DetailComponent } from './detail/detail.component';
import { PoComponent } from './po-list/po.component';
import { PoListT3Component } from './po-list-t3/po-list-t3.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ByRackRoutingModule,
    NgxSpinnerModule,
    PaginationModule
  ],
  declarations: [
    MainComponent,
    DetailComponent,
    PoComponent,
    PoListT3Component
  ]
})
export class ByRackModule {}
