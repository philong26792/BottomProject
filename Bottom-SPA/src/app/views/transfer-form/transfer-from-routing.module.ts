import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { GenerateCollectionComponent } from './generate-collection/generate-collection.component';
import { PrintComponent } from './print/print.component';
import { PrintFormComponent } from './print-form/print-form.component';
import { TransferFormPrintNavGuard } from '../../_core/_guards/transfer-form-print-nav.guard';
import { TransferFormGenerateNavGuard } from '../../_core/_guards/transfer-form-generate-nav.guard';

const routes: Routes = [
    {
        canActivate: [TransferFormGenerateNavGuard],
        path: 'genrate',
        component: GenerateCollectionComponent,
        data: {
            title: 'Generate collections'
        }

    },
    {
        canActivate: [TransferFormPrintNavGuard],
        path: 'print',
        component: PrintComponent,
        data: {
            title: 'Print'
        }

    },
    {
        canActivate: [TransferFormPrintNavGuard],
        path: 'print-form',
        component: PrintFormComponent,
        data: {
            title: 'Print Form'
        }

    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class TransferFormRoutingModule {
}
