import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingT2SupplierRoutingModule } from './setting-t2-supplier-routing.module';
import { AddT2SupplierComponent } from './add-t2-supplier/add-t2-supplier.component';
import { ListT2SupplierComponent } from './list-t2-supplier/list-t2-supplier.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelect2Module } from 'ng-select2';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';



@NgModule({
  declarations: [AddT2SupplierComponent, ListT2SupplierComponent],
  imports: [
    CommonModule,
    SettingT2SupplierRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    NgSelect2Module,
    NgxSpinnerModule,
    PaginationModule.forRoot()
  ],

})
export class SettingT2SupplierModule { }
