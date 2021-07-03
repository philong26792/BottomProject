import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { ReceivingMaterialBNavGuard } from '../../_core/_guards/receiving-material-b-nav.guard';
import { ReceiveMainComponent } from "./receive-main/receive-main.component";
import { RecordDetailComponent } from './record-detail/record-detail.component';
import { RecordComponent } from './record/record.component';

const routes: Routes = [
  {
    path: "",
    canActivate: [ReceivingMaterialBNavGuard],
    data: {
      title: "Receiving Material",
    },
    children: [
      {
        path: "main",
        component: ReceiveMainComponent,
        data: {
          title: "Receive",
        },
      },
      {
        path: "record",
        component: RecordComponent,
        data: {
          title: "Record",
        },
      },
      {
        path: "record-detail",
        component: RecordDetailComponent,
        data: {
          title: "Detail Record",
        },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReceiveEnoughRoutingModule {}
