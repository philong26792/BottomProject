import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { RackMainComponent } from './rack-main/rack-main.component';
import { RackListResolver } from '../../_core/_resolvers/rack-list.resolver';
import { RackFormComponent } from './rack-form/rack-form.component';
import { RackPrintComponent } from './rack-print/rack-print.component';
import { RackLocationNavNavGuard } from '../../_core/_guards/rack-location-nav.guard';
import { RackPrintOneComponent } from './rack-print-one/rack-print-one.component';



const routes: Routes = [
    {
        path: '',
        canActivate: [RackLocationNavNavGuard],
        data: {
            title: 'rack'
        },
        children: [
            {
                path: 'main',
                component: RackMainComponent,
                resolve: { racks: RackListResolver },
                data: {
                    title: 'Rack Location'
                }

            },
            {
                path: 'form',
                component: RackFormComponent,
                data: {
                    title: 'Rack Location'
                }

            },
            {
                path: 'print',
                component: RackPrintComponent,
                data: {
                    title: 'Rack Location'
                }

            },
            {
                path: 'print-one',
                component: RackPrintOneComponent,
                data: {
                    title: 'Rack Location'
                }

            },
            
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class RackRoutingModule {
}
