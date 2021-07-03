import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { LocationStrategy, HashLocationStrategy, CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { NgxQRCodeModule } from 'ngx-qrcode2';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

import { AppComponent } from './app.component';

// Import containers
import { DefaultLayoutComponent } from './containers';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/login/login.component';

const APP_CONTAINERS = [
  DefaultLayoutComponent
];

import {
  AppAsideModule,
  AppBreadcrumbModule,
  AppHeaderModule,
  AppFooterModule,
  AppSidebarModule,
} from '@coreui/angular';

// Import routing module
import { AppRoutingModule } from './app.routing';

// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ChartsModule } from 'ng2-charts';
import { NgSelectModule } from '@ng-select/ng-select';


// Import service
import { AuthService } from '../app/_core/_services/auth.service';
import { ErrorInterceptorProvider } from './_core/_services/error.interceptor';
import { AlertifyService } from './_core/_services/alertify.service';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { JwtModule } from '@auth0/angular-jwt';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxPrintModule } from 'ngx-print';
import { RackListResolver } from './_core/_resolvers/rack-list.resolver';
import { CheckLoginComponent } from './views/check-login/check-login.component';
import { AuthGuard } from './_core/_guards/auth.guard';
import { InputNavGuard } from './_core/_guards/input-nav.guard';
import { OutputNavGuard } from './_core/_guards/output-nav.guard';
import { QrGenerateNavGuard } from './_core/_guards/qr-generate-nav.guard';
import { RackLocationNavNavGuard } from './_core/_guards/rack-location-nav.guard';
import { ReceivingMaterialNavGuard } from './_core/_guards/receiving-material-nav.guard';
import { TransferLocationNavGuard } from './_core/_guards/transfer-loaction-nav.guard';
import { KabanLayoutComponent } from './containers/kaban-layout/kaban-layout.component';
import { MainComponent } from './views/kanban/main/main.component';
import { ReportNavGuard } from './_core/_guards/report-nav.guard';
import { KanbanNavGuard } from './_core/_guards/kanban-nav.guard';
import { HistoryNavGuard } from './_core/_guards/history-nav.guard';
import { commonPerFactory } from './_core/_utility/common-per-factory';
import { SettingMailNavNavGuard } from './_core/_guards/setting-mail-nav.guard';
import { TransferFormPrintNavGuard } from './_core/_guards/transfer-form-print-nav.guard';
import { TransferFormGenerateNavGuard } from './_core/_guards/transfer-form-generate-nav.guard';
import { ReceivingMaterialBNavGuard } from './_core/_guards/receiving-material-b-nav.guard';
import { QrGeneratePrintNavGuard } from './_core/_guards/qr-generate-print-nav.guard';
import { QrGenerateIntegrationInputNavGuard } from './_core/_guards/qr-generate-integration-input-nav.guard';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  imports: [
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    AppAsideModule,
    AppBreadcrumbModule.forRoot(),
    AppFooterModule,
    AppHeaderModule,
    AppSidebarModule,
    PerfectScrollbarModule,
    NgSelectModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    PaginationModule.forRoot(),
    ChartsModule,
    NgxSpinnerModule,
    NgxQRCodeModule,
    NgxPrintModule,
    BsDatepickerModule.forRoot(),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: [commonPerFactory.serverSentTokenInAppModule],
        blacklistedRoutes: [commonPerFactory.linkSentTokenInAppModule]
      }
    })
  ],
  declarations: [
    AppComponent,
    ...APP_CONTAINERS,
    P404Component,
    P500Component,
    LoginComponent,
    CheckLoginComponent,
    KabanLayoutComponent,
    MainComponent
  ],
  providers: [
    AuthService,
    ErrorInterceptorProvider,
    AlertifyService,
    AuthGuard,
    InputNavGuard,
    OutputNavGuard,
    QrGenerateNavGuard,
    QrGeneratePrintNavGuard,
    QrGenerateIntegrationInputNavGuard,
    RackLocationNavNavGuard,
    ReceivingMaterialNavGuard,
    TransferLocationNavGuard,
    ReportNavGuard,
    KanbanNavGuard,
    HistoryNavGuard,
    SettingMailNavNavGuard,
    TransferFormPrintNavGuard,
    TransferFormGenerateNavGuard,
    ReceivingMaterialBNavGuard,
    RackListResolver,
    {
      provide: LocationStrategy,
      useClass: HashLocationStrategy
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
