import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { MainComponent } from './main/main.component';
import { DetailComponent } from './detail/detail.component';
import { DetailByReceivingTypeComponent } from './detail-by-receiving-type/detail-by-receiving-type.component';


const routes: Routes = [
    {
        path: 'main',
        component: MainComponent,
        data: {
            title: 'Kaban By Category'
        },
    },
    {
        path: 'detail',
        component: DetailComponent,
        data: {
            title: 'Kanban'
        }
    },
    {
        path: 'detail-by-receiving-type',
        component: DetailByReceivingTypeComponent,
        data: {
            title: 'Kanban'
        }
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ByPoRoutingModule { }
