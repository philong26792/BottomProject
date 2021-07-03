import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { ListMailComponent } from './list-mail/list-mail.component';
import { AddMailComponent } from './add-mail/add-mail.component';
import { SettingMailNavNavGuard } from '../../_core/_guards/setting-mail-nav.guard';


const routes: Routes = [

    {
        canActivate: [SettingMailNavNavGuard],
        path: 'list-mail',
        component: ListMailComponent,
        data: {
            title: 'List mail'
        }

    },
    {
        canActivate: [SettingMailNavNavGuard],
        path: 'add-mail',
        component: AddMailComponent,
        data: {
            title: 'Add Mail'
        }

    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})

export class SettingMailRoutingModule {
}
