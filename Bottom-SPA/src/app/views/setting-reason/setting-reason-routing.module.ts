import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AddReasonComponent } from "./add-reason/add-reason.component";
import { ListReasonComponent } from "./list-reason/list-reason.component";

const routes: Routes = [
    {
        path: 'list-reason',
        component: ListReasonComponent,
        data: {
            title: 'List Reason'
        }
    },
    {
        path: 'add-reason',
        component: AddReasonComponent,
        data: {
            title: 'Add Reason'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SettingReasonRoutingModule { }