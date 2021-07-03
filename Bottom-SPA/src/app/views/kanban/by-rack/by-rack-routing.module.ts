import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { MainComponent } from './main/main.component';
import { DetailComponent } from './detail/detail.component';
import { PoComponent } from './po-list/po.component';
import { PoListT3Component } from './po-list-t3/po-list-t3.component';

const routes: Routes = [
    {
        path: 'main',
        component: MainComponent,
        data: {
            title: 'Kaban By Category'
        },
    },
    {
        path: 'detail/:codeId',
        component: DetailComponent,
        data: {
            title: 'Kanban'
        }
    },
    {
        path: 'po-list/:codeId/:rack',
        component: PoComponent,
        data: {
            title: 'Po List'
        }

    },
    {
        path: 'po-list-t3',
        component: PoListT3Component,
        data: {
            title: 'Po List'
        }

    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ByRackRoutingModule { }
