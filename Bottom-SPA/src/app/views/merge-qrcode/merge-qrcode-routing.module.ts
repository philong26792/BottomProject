import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MergeQrcodeMainComponent } from './merge-qrcode-main/merge-qrcode-main.component';
import { OtherSplitDetailComponent } from './other-split-detail/other-split-detail.component';
import { OtherSplitMainComponent } from './other-split-main/other-split-main.component';
import { OtherSplitProcessComponent } from './other-split-process/other-split-process.component';
import { QrcodeAfterMergeComponent } from './qrcode-after-merge/qrcode-after-merge.component';
import { QrcodeDetailComponent } from './qrcode-detail/qrcode-detail.component';
import { SplitQrcodeDetailComponent } from './split-qrcode-detail/split-qrcode-detail.component';
import { SplitDetailComponent } from './split-detail/split-detail.component';
import { SplitMainComponent } from './split-main/split-main.component';
import { SplitProcessComponent } from './split-process/split-process.component';
import { OtherSplitQrcodeDetailComponent } from './other-split-qrcode-detail/other-split-qrcode-detail.component';
import { OtherSplitQrcodeDetailChildComponent } from './other-split-qrcode-detail-child/other-split-qrcode-detail-child.component';
import { SplitQrcodeDetailChildComponent } from './split-qrcode-detail-child/split-qrcode-detail-child.component';


const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Merge QRCode'
    },
    children: [
      {
        path: 'main',
        component: MergeQrcodeMainComponent,
        data: {
          title: 'Main'
        }
      },
      {
        path: 'after',
        component: QrcodeAfterMergeComponent,
        data: {
          title: 'QRCode After Merge'
        }
      },
      {
        path: 'qrcode-detail',
        component: QrcodeDetailComponent,
        data: {
          title: 'QRCode Detail'
        }
      },
      {
        path: 'split/main',
        component: SplitMainComponent,
        data: {
          title: 'Split Main'
        }
      },
      {
        path: 'split/process/:transacNo',
        component: SplitProcessComponent,
        data: {
          title: 'Split Process'
        }
      },
      {
        path: 'split/detail/:transacNo',
        component: SplitDetailComponent,
        data: {
          title: 'Split Detail'
        }
      },
      {
        path: 'split/qrcode-detail/:transacNo',
        component: SplitQrcodeDetailComponent,
        data: {
          title: 'QrCode Split Detail'
        }
      },
      {
        path: 'split/qrcode-detail-child/:transacNo',
        component: SplitQrcodeDetailChildComponent,
        data: {
          title: 'QrCode Split Detail Child'
        }
      },
      {
        path: 'other-split/main',
        component: OtherSplitMainComponent,
        data: {
          title: 'Other Split Main'
        }
      },
      {
        path: 'other-split/process/:transacNo',
        component: OtherSplitProcessComponent,
        data: {
          title: 'Other Split Process'
        }
      },
      {
        path: 'other-split/detail/:transacNo',
        component: OtherSplitDetailComponent,
        data: {
          title: 'Other Split Detail'
        }
      },
      {
        path: 'other-split/qrcode-detail/:transacNo',
        component: OtherSplitQrcodeDetailComponent,
        data: {
          title: 'QrCode Other Split Detail'
        }
      },
      {
        path: 'other-split/qrcode-detail-child/:transacNo',
        component: OtherSplitQrcodeDetailChildComponent,
        data: {
          title: 'QrCode Other Split Detail Child'
        }
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MergeQrcodeRoutingModule { }
