import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ByBatchInComponent } from './by-batch-in/by-batch-in.component';
import { ByBatchOutComponent } from './by-batch-out/by-batch-out.component';
import { DetailQrcodeComponent } from './detail-qrcode/detail-qrcode.component';
import { ListQrcodeChangeComponent } from './list-qrcode-change/list-qrcode-change.component';
import { ModifyQrcodeMainComponent } from './modify-qrcode-main/modify-qrcode-main.component';
import { NoByBatchInComponent } from './no-by-batch-in/no-by-batch-in.component';
import { NoByBatchOutComponent } from './no-by-batch-out/no-by-batch-out.component';
import { PrintListMissingComponent } from './print-list-missing/print-list-missing.component';


const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Stock Adj'
    },
    children: [
      {
        path: 'main',
        component: ModifyQrcodeMainComponent,
        data: {
          title: 'Main'
        }
      },
      {
        path: 'no-by-batch-out',
        component: NoByBatchOutComponent,
        data: {
          title: 'No By Batch Out'
        }
      },
      {
        path: 'no-by-batch-in',
        component: NoByBatchInComponent,
        data: {
          title: 'No By Batch In'
        }
      },
      {
        path: 'by-batch-in',
        component: ByBatchInComponent,
        data: {
          title: 'By Batch In'
        }
      },
      {
        path: 'by-batch-out',
        component: ByBatchOutComponent,
        data: {
          title: 'By Batch Out'
        }
      },
      {
        path: 'list-qrcode-change',
        component: ListQrcodeChangeComponent,
        data: {
          title: 'List QrCode Change'
        }
      },
      {
        path: 'detail-qrcode',
        component: DetailQrcodeComponent,
        data: {
          title: 'Detail QrCode'
        }
      },
      {
        path: 'print-list-missing',
        component: PrintListMissingComponent,
        data: {
          title: 'Print Missing'
        }
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockAdjRoutingModule { }
