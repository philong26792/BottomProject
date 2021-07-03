import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { QrMainComponent } from './qr-main/qr-main.component';
import { QrPrintComponent } from './qr-print/qr-print.component';
import { QrBodyComponent } from './qr-body/qr-body.component';
import { QrGenerateNavGuard } from '../../_core/_guards/qr-generate-nav.guard';
import { IntegrationInputComponent } from './integration-input/integration-input.component';
import { QrGeneratePrintNavGuard } from '../../_core/_guards/qr-generate-print-nav.guard';
import { QrGenerateIntegrationInputNavGuard } from '../../_core/_guards/qr-generate-integration-input-nav.guard';

const routes: Routes = [
    {
        path: '',
        data: {
            title: 'QR Generate'
        },
        children: [
            {
                path: 'main',
                canActivate: [QrGenerateNavGuard],
                component: QrMainComponent,
                data: {
                    title: 'Search'
                }

            },
            {
                path: 'print',
                component: QrPrintComponent,
                data: {
                    title: 'QR Print'
                }
            },
            {
                path: 'body',
                canActivate: [QrGeneratePrintNavGuard],
                component: QrBodyComponent,
                data: {
                    title: 'QR Body'
                }
            },
            {
                path: 'integration',
                canActivate: [QrGenerateIntegrationInputNavGuard],
                component: IntegrationInputComponent,
                data: {
                    title: 'Integration Input'
                }
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class QrRoutingModule {
}
