import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

import { TransferMainComponent } from './transfer-main/transfer-main.component';
import { TransferHistoryComponent } from './transfer-history/transfer-history.component';
import { TransferDetailComponent } from './transfer-detail/transfer-detail.component';
import { TransferLocationNavGuard } from '../../_core/_guards/transfer-loaction-nav.guard';
import { HistoryNavGuard } from '../../_core/_guards/history-nav.guard';

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'Transfer'
        },
        children: [
            {
                path: 'main',
                canActivate: [TransferLocationNavGuard],
                component: TransferMainComponent,
                data: {
                    title: 'Transfer Main'
                }

            },
            {
                path: 'history',
                canActivate: [HistoryNavGuard],
                component: TransferHistoryComponent,
                data: {
                    title: 'Transfer History'
                }

            },
            {
                path: 'detail/:transferType/:transferNo',
                component: TransferDetailComponent,
                data: {
                    title: 'Transfer Detail'
                }

            },
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class TransferRoutingModule {
}
