import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AddT2SupplierComponent } from "./add-t2-supplier/add-t2-supplier.component";
import { ListT2SupplierComponent } from "./list-t2-supplier/list-t2-supplier.component";


const routes: Routes = [
    {
        path: 'list-t2-supplier',
        component: ListT2SupplierComponent,
        data: {
            title: 'List T2 Supplier'
        }
    },
    {
        path: 'add-t2-supplier',
        component: AddT2SupplierComponent,
        data: {
            title: 'Add T2 Supplier'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SettingT2SupplierRoutingModule { }