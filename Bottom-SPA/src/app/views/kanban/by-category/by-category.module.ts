import { NgxSpinnerModule } from 'ngx-spinner';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MainComponent } from './main/main.component';
import { ByCategoryRoutingModule } from './by-category-routing.module';
import { DetailComponent } from './detail/detail.component';
import { DetailPoNoComponent } from './detail-po-no/detail-po-no.component';
import { DetailToolCodeComponent } from './detail-tool-code/detail-tool-code.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ByCategoryRoutingModule,
    NgxSpinnerModule,
    PaginationModule
  ],
  declarations: [
    MainComponent,
    DetailComponent,
    DetailPoNoComponent,
    DetailToolCodeComponent
  ]
})
export class ByCategoryModule {}
