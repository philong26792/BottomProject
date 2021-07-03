import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddMailComponent } from './add-mail/add-mail.component';
import { ListMailComponent } from './list-mail/list-mail.component';
import { SettingMailRoutingModule } from './setting-mail-routing.module';
import { NgSelect2Module } from 'ng-select2';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@NgModule({
  imports: [
    CommonModule,
    NgSelect2Module,
    FormsModule,
    ReactiveFormsModule,
    SettingMailRoutingModule,
    NgSelect2Module,
    NgxSpinnerModule,
    PaginationModule.forRoot(),
  ],
  declarations: [
    AddMailComponent,
    ListMailComponent
  ]
})
export class SettingMailModule { }
