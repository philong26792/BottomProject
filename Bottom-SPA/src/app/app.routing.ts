import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';
import { AuthGuard } from './_core/_guards/auth.guard';
import { CheckLoginComponent } from './views/check-login/check-login.component';
import { KabanLayoutComponent } from './containers/kaban-layout/kaban-layout.component';
import { MainComponent } from './views/kanban/main/main.component';
import { KanbanNavGuard } from './_core/_guards/kanban-nav.guard';

export const routes: Routes = [
  {
    path: "",
    canActivate: [AuthGuard],
    redirectTo: "dashboard",
    pathMatch: "full"
  },
  {
    path: "404",
    component: P404Component,
    data: {
      title: "Page 404"
    }
  },
  {
    path: "500",
    component: P500Component,
    data: {
      title: "Page 500"
    }
  },
  {
    path: "login",
    component: LoginComponent,
    data: {
      title: "Login Page"
    }
  },
  {
    path: 'CheckLogin',
    component: CheckLoginComponent,
  },
  {
    path: "",
    canActivate: [AuthGuard],
    component: DefaultLayoutComponent,
    data: {
      title: "Home"
    },
    children: [
      {
        path: "dashboard",
        loadChildren: () =>
          import("./views/dashboard/dashboard.module").then(
            m => m.DashboardModule
          )
      },
      {
        path: "receipt",
        loadChildren: () =>
          import("./views/receipt/receipt.module").then(
            m => m.ReceiptModule
          )
      },
      {
        path: "receive",
        loadChildren: () =>
          import("./views/receive-enough/receive-enough.module").then(
            m => m.ReceiveEnoughModule
          )
      },
      {
        path: "qr",
        loadChildren: () =>
          import("./views/qr-generate/qr.module").then(
            m => m.QrModule
          )
      },
      {
        path: "input",
        loadChildren: () =>
          import("./views/input/input.module").then(
            m => m.InputModule
          )
      },
      {
        path: "output",
        loadChildren: () =>
          import("./views/output/output.module").then(
            m => m.OutputModule
          )
      },
      {
        path: "rack",
        loadChildren: () =>
          import("./views/rack-location/rack.module").then(
            m => m.RackModule
          )
      },
      {
        path: 'transfer',
        loadChildren: () =>
          import('./views/transfer-location/transfer.module').then(
            m => m.TransferModule
          )
      },
      {
        path: 'report',
        loadChildren: () =>
          import('./views/report/report.module').then(
            m => m.ReportModule
          )
      },
      {
        path: 'input/transfer-form',
        loadChildren: () =>
          import('./views/transfer-form/transfer-form.module').then(
            m => m.TransferFormModule
          )
      },
      {
        path: 'rack/setting-mail',
        loadChildren: () =>
          import('./views/setting-mail/setting-mail.module').then(
            m => m.SettingMailModule
          )
      },
      {
        path: 'rack/setting-reason',
        loadChildren: () =>
          import('./views/setting-reason/setting-reason.module').then(
            m => m.SettingReasonModule)
      },
      {
        path: 'rack/setting-t2-supplier',
        loadChildren: () =>
          import('./views/setting-T2-supplier/setting-t2-supplier.module').then(
            m => m.SettingT2SupplierModule)
      },
      {
        path: 'stock-adj',
        loadChildren: () =>
          import('./views/stock-adj/stock-adj.module').then(
            m => m.StockAdjModule
          )
      },
      {
        path: 'modify-store',
        loadChildren: () =>
          import('./views/modify-qrcode/modify-qrcode.module').then(
            m => m.ModifyQrcodeModule
          )
      },
      {
        path: 'merge-qrcode',
        loadChildren: () =>
          import('./views/merge-qrcode/merge-qrcode.module').then(
            m => m.MergeQrcodeModule
          )
      }
    ]
  },
  {
    path: 'kanban',
    component: KabanLayoutComponent,
    canActivate: [AuthGuard, KanbanNavGuard],
    data: {
      title: 'Kanban'
    },
    children: [
      {
        path: '',
        component: MainComponent
      },
      {
        path: 'by-category',
        loadChildren: () =>
          import('./views/kanban/by-category/by-category.module').then(m => m.ByCategoryModule)
      },
      {
        path: 'by-po',
        loadChildren: () =>
          import('./views/kanban/by-po/by-po.module').then(m => m.ByPoModule)
      },
      {
        path: 'by-rack',
        loadChildren: () =>
          import('./views/kanban/by-rack/by-rack.module').then(m => m.ByRackModule)
      },
    ]
  },
  { path: "**", component: P404Component }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
