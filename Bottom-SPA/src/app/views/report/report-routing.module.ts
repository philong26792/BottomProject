import { RouterModule, Routes } from "@angular/router";
import { NgModule } from "@angular/core";
import { ReceiveMaterialComponent } from "./receive-material/receive-material.component";
import { ReportNavGuard } from "../../_core/_guards/report-nav.guard";
import { CompareReportComponent } from './compare-report/compare-report.component';

const routes: Routes = [
    {
        path: "",
        data: {
            title: "Report",
        },
        children: [
            {
                path: "receive-material",
                canActivate: [ReportNavGuard],
                component: ReceiveMaterialComponent,
                data: {
                    title: "Receive Material",
                },
            },
            {
                path: "compare-report",
                component: CompareReportComponent,
                data: {
                    title: "Compare Report",
                },
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class ReportRoutingModule { }
