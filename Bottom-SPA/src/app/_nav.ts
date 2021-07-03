import { INavData } from '@coreui/angular';
import { Injectable } from '@angular/core';

export const navItems: INavData[] = [];

@Injectable({
  providedIn: 'root'  // <- ADD THIS
})
export class NavItem {
  navItems: INavData[] = [];
  hasMaintain: boolean;
  hasTransaction: boolean;
  hasKanban: boolean;
  hasReport: boolean;
  hasQuery: boolean;
  hasReceiving: boolean;
  constructor() { }

  getNav() {
    this.navItems = [];
    const user: any = JSON.parse(localStorage.getItem('user'));

    //#region "navMaintain"
    const navMaintain = {
      name: '1. Maintain',
      url: '/rack',
      icon: 'fa fa-cogs',
      children: []
    };
    const navMaintain_RackLocation = {
      name: '1.1 Rack Location',
      url: '/rack/main',
      class: 'menu-margin',
    };
    const navMaintain_T3Supplier = {
      name: '1.2 T3 Supplier',
      url: '/rack/setting-mail/list-mail',
      class: 'menu-margin',
    };
    const navMaintain_SettingReason = {
      name: '1.3 Reason For Change',
      url: '/rack/setting-reason/list-reason',
      class: 'menu-margin',
    };
    const navMaintain_SettingT2Supplier = {
      name: '1.4 Setting T2 Supplier',
      url: '/rack/setting-t2-supplier/list-t2-supplier',
      class: 'menu-margin',
    };
    if (user.role.includes('wmsb.RackLocationMain') === true) {
      navMaintain.children.push(navMaintain_RackLocation);
      this.hasMaintain = true;
    }
    if (user.role.includes('wmsb.SettingT3Supplier') === true) {
      navMaintain.children.push(navMaintain_T3Supplier);
      this.hasMaintain = true;
    }

    if (user.role.includes('wmsb.ReasonForChange') === true) {
      navMaintain.children.push(navMaintain_SettingReason);
      this.hasMaintain = true;
    }
    if (user.role.includes('wmsb.SettingT3Supplier') === true) {
      navMaintain.children.push(navMaintain_SettingT2Supplier);
      this.hasMaintain = true;
    }
    //#endregion

    //#region "navTransaction"
    const navTransaction = {
      name: '2. Transaction',
      url: '/transaction',
      icon: 'fa fa-balance-scale',
      children: []
    };
    const navTransaction_Receiving = {
      name: '2.1 Receiving Material',
      url: 'receiving',
      class: 'menu-margin',
      children: []
    };
    const navTransaction_Receiving_A = {
      name: '2.1.1 Receiving Mat# A',
      url: '/receipt/main',
      class: 'menu-child-margin',
    };
    const navTransaction_Receiving_B = {
      name: '2.1.2 Receiving Mat# B',
      url: '/receive/main',
      class: 'menu-child-margin',
    };
    const navTransaction_QrGeneratePrint = {
      name: '2.2 QR Code Generate',
      url: '/qr/main',
      class: 'menu-margin',
    };
    const navTransaction_IntegrationInput = {
      name: '2.3 QR Generate (Integration)',
      url: '/qr/integration',
      class: 'menu-margin',
    };
    const navTransaction_InputMain = {
      name: '2.4 Input',
      url: '/input/main',
      class: 'menu-margin',
    };
    const navTransaction_GenerateCollectionTransferForm = {
      name: '2.5 Transfer Form Generate',
      url: '/input/transfer-form/genrate',
      class: 'menu-margin',
    };
    const navTransaction_Output = {
      name: '2.6 Out',
      url: '/output/main',
      class: 'menu-margin',
    };
    const navTransaction_TransferLocationMain = {
      name: '2.7 Transfer Location',
      url: '/transfer/main',
      class: 'menu-margin',
    };
    const navTransaction_Modify_Qrcode = {
      name: '2.8 Stock Adj.',
      url: '/modify-store/main',
      class: 'menu-margin',
      children: []
    };
    const navTransaction_Modify_Qrcode_Stock_Adj1 = {
      name: '2.8.1 Stock Adj. 1',
      url: '/stock-adj/main',
      class: 'menu-child-margin',
    };
    const navTransaction_Modify_Qrcode_Stock_Adj2 = {
      name: '2.8.2 Stock Adj. 2',
      url: '/modify-store/main',
      class: 'menu-child-margin',
    };
    //--------------Merge QrCode--------------------//
    const navTransaction_Merge_Qrcode = {
      name: '2.9 Merge & Split',
      url: 'merge-qrcode',
      class: 'menu-margin',
      children: []
    };
    let hasMergeSplit;
    const navTransaction_Merge_Qrcode_Main = {
      name: '2.9.1 Merge QRCode',
      url: '/merge-qrcode/main',
      class: 'menu-child-margin',
    };
    const navTransaction_Merge_Qrcode_Split = {
      name: '2.9.2 Offset A ->a1,a2…',
      url: '/merge-qrcode/split/main',
      class: 'menu-child-margin',
    };
    const navTransaction_Merge_Qrcode_Other_Split = {
      name: '2.9.3 Offset A ->b1,b2…',
      url: '/merge-qrcode/other-split/main',
      class: 'menu-child-margin',
    };

    if (user.role.includes('wmsb.ReceivingMaterial.A') === true) {
      navTransaction_Receiving.children.push(navTransaction_Receiving_A);
      this.hasReceiving = true;
    }
    if (user.role.includes('wmsb.ReceivingMaterial.B') === true) {
      navTransaction_Receiving.children.push(navTransaction_Receiving_B);
      this.hasReceiving = true;
    }
    if (this.hasReceiving) {
      navTransaction.children.push(navTransaction_Receiving);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.QrGeneratePrint') === true) {
      navTransaction.children.push(navTransaction_QrGeneratePrint);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.IntegrationInput') === true) {
      navTransaction.children.push(navTransaction_IntegrationInput);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.InputMain') === true) {
      navTransaction.children.push(navTransaction_InputMain);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.GenerateCollectionTransferForm') === true) {
      navTransaction.children.push(navTransaction_GenerateCollectionTransferForm);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.Output') === true) {
      navTransaction.children.push(navTransaction_Output);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.TransferLocationMain') === true) {
      navTransaction.children.push(navTransaction_TransferLocationMain);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.StockAdj') === true) {
      navTransaction_Modify_Qrcode.children.push(navTransaction_Modify_Qrcode_Stock_Adj1);
      navTransaction_Modify_Qrcode.children.push(navTransaction_Modify_Qrcode_Stock_Adj2);
      navTransaction.children.push(navTransaction_Modify_Qrcode);
      this.hasTransaction = true;
    }
    if (user.role.includes('wmsb.MergeQRCode') === true) {
      navTransaction_Merge_Qrcode.children.push(navTransaction_Merge_Qrcode_Main);
      hasMergeSplit = true;
    }
    if (user.role.includes('wmsb.OffsetAa1a2') === true) {
      navTransaction_Merge_Qrcode.children.push(navTransaction_Merge_Qrcode_Split);
      hasMergeSplit = true;
    }
    if (user.role.includes('wmsb.OffsetAb1b2') === true) {
      navTransaction_Merge_Qrcode.children.push(navTransaction_Merge_Qrcode_Other_Split);
      hasMergeSplit = true;
    }
    if (hasMergeSplit) {
      navTransaction.children.push(navTransaction_Merge_Qrcode);
      this.hasTransaction = true;
    }
    //#endregion

    //#region "navKanban"
    const navKanban = {
      name: '3. Kanban',
      url: '/kanban',
      icon: 'fa fa-desktop',
      children: []
    };
    const navKanban_Kanban = {
      name: '3.1 Kanban',
      url: '/kanban/',
      class: 'menu-margin',
      attributes: { target: '_blank' },
    };
    if (user.role.includes('wmsb.Kanban') === true) {
      navKanban.children.push(navKanban_Kanban);
      this.hasKanban = true;
    }
    //#endregion

    //#region "navReport"
    const navReport = {
      name: '4. Report',
      url: '/report',
      icon: 'fa fa-newspaper-o',
      children: []
    };
    const navReport_Report = {
      name: '4.1 Receive Material Report',
      url: '/report/receive-material',
      class: 'menu-margin',
    };
    if (user.role.includes('wmsb.ReceiveMaterialReport') === true) {
      navReport.children.push(navReport_Report);
      this.hasReport = true;
    }
    //#endregion

    //#region "navQuery"
    const navQuery = {
      name: '5. Query',
      url: '/query',
      icon: 'fa fa-search',
      children: []
    };
    const navQuery_History = {
      name: '5.1 History',
      url: '/transfer/history',
      class: 'menu-margin',
    };
    const navQuery_QrGeneratePrint = {
      name: '5.2 Material Form Print',
      url: '/qr/body',
      class: 'menu-margin',
    };
    const navQuery_QrPrintAgain = {
      name: '5.3 Sorting Form Print',
      url: '/input/qrcode-again',
      class: 'menu-margin',
    };
    const navQuery_MissingPrint = {
      name: '5.4 Missing Print',
      url: '/input/missing-again',
      class: 'menu-margin',
    };
    const navQuery_PrintTransferForm = {
      name: '5.5 Transfer Form (Email/Print/Release)',
      url: '/input/transfer-form/print',
      class: 'menu-margin',
    };
    const navQuery_CompareReport = {
      name: "5.6 Compare Report",
      url: "/report/compare-report",
      class: "menu-margin",
    };
    if (user.role.includes('wmsb.InOutHistory') === true) {
      navQuery.children.push(navQuery_History);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.QrGeneratePrint') === true) {
      navQuery.children.push(navQuery_QrGeneratePrint);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.QrPrintAgain') === true) {
      navQuery.children.push(navQuery_QrPrintAgain);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.MissingPrint') === true) {
      navQuery.children.push(navQuery_MissingPrint);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.PrintTransferForm') === true) {
      navQuery.children.push(navQuery_PrintTransferForm);
      this.hasQuery = true;
    }
    if (user.role.includes('wmsb.CompareReport') === true) {
      navQuery.children.push(navQuery_CompareReport);
      this.hasQuery = true;
    }

    //#endregion

    if (this.hasMaintain) {
      this.navItems.push(navMaintain);
    }
    if (this.hasTransaction) {
      this.navItems.push(navTransaction);
    }
    if (this.hasKanban) {
      this.navItems.push(navKanban);
    }
    if (this.hasReport) {
      this.navItems.push(navReport);
    }
    if (this.hasQuery) {
      this.navItems.push(navQuery);
    }
    return this.navItems;
  }
}
