import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ListReasonComponent } from './list-reason/list-reason.component';
import { AddReasonComponent } from './add-reason/add-reason.component';
import { SettingReasonRoutingModule } from "./setting-reason-routing.module";
import { PaginationModule } from "ngx-bootstrap/pagination";
import { NgxSpinnerModule } from "ngx-spinner";
import { NgSelect2Module } from "ng-select2";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        SettingReasonRoutingModule,
        NgSelect2Module,
        NgxSpinnerModule,
        PaginationModule.forRoot(),
    ],
    declarations: [
        ListReasonComponent,
        AddReasonComponent
    ]
})
export class SettingReasonModule { }