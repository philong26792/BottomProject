import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { MainComponent } from './main/main.component';
import { DetailComponent } from './detail/detail.component';
import { DetailToolCodeComponent } from './detail-tool-code/detail-tool-code.component';
import { DetailPoNoComponent } from './detail-po-no/detail-po-no.component';


const routes: Routes = [
  {
    path: "main",
    component: MainComponent,
    data: {
      title: "Kaban By Category",
    },
  },
  {
    path: "detail/:codeId",
    component: DetailComponent,
    data: {
      title: "KanBan",
    },
  },
  {
    path: "detail/tool-code/:codeId/:toolCode",
    component: DetailToolCodeComponent,
    data: {
      title: "KanBan",
    },
  },
  {
    path: "detail/po-no/:codeId/:toolCode/:po",
    component: DetailPoNoComponent,
    data: {
      title: "KanBan",
    },
  },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class ByCategoryRoutingModule { }
