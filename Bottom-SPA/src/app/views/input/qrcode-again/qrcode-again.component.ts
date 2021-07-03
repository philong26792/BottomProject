import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InputService } from '../../../_core/_services/input.service';
import { TransactionMain } from '../../../_core/_models/transaction-main';
import { Pagination, PaginatedResult } from '../../../_core/_models/pagination';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { FilterQrCodeAgainParam } from '../../../_core/_viewmodels/qrcode-again-search';
import { PackingPrintAll } from '../../../_core/_viewmodels/packing-print-all';
import { NgxSpinnerService } from 'ngx-spinner';
import { OutputService } from '../../../_core/_services/output.service';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Select2OptionData } from 'ng-select2';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { QrCodeAgainModel } from '../../../_core/_models/qrcode-again-model';
@Component({
  selector: 'app-qrcode-again',
  templateUrl: './qrcode-again.component.html',
  styleUrls: ['./qrcode-again.component.scss']
})
export class QrcodeAgainComponent implements OnInit {
  disable : false;
  time_start: string;
  time_end: string;
  bsConfig: Partial<BsDatepickerConfig>;
  supplier_ID: string;
  public supplierList: Array<Select2OptionData>;
  mO_No: string;
  rack_Location: string;
  material_ID: string;
  material_Name: string;
  pagination: Pagination;
  qrCodeAgainParam: FilterQrCodeAgainParam;
  transactionMainList: QrCodeAgainModel[] = [];
  transactionMainListAll: QrCodeAgainModel[] = [];
  checkBoxAll: boolean = false;
  packingPrintAll: PackingPrintAll[] = [];
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%",
  };
  alerts: any = [
    {
      type: 'success',
      msg: `You successfully read this important alert message.`
    },
    {
      type: 'info',
      msg: `This alert needs your attention, but it's not super important.`
    },
    {
      type: 'danger',
      msg: `Better check yourself, you're not looking too good.`
    }
  ];

  checkPrintLocation: string = 'true';
  listParamPrintQrCodeAgain: Array<any> = [];

  constructor(private inputService: InputService,
              private router: Router,
              private functionUtility: FunctionUtility,
              private spinnerService: NgxSpinnerService,
              private packingListService: PackingListService,
              private alertifyService: AlertifyService,
              private outputService: OutputService) { }

  ngOnInit() {
    this.getSupplier();
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 5,
      totalItems: 0,
      totalPages: 0
    };
    this.getTimeNow();
    this.checkPrintLocationInLocalStorage();
    this.inputService.currentQrCodeAgainParam.subscribe(res => this.qrCodeAgainParam = res);
    if (this.qrCodeAgainParam === null) {
      this.getTimeNow();
    } else {
      this.time_start = this.qrCodeAgainParam.from_Date;
      this.time_end = this.qrCodeAgainParam.to_Date;
      this.mO_No = this.qrCodeAgainParam.mO_No;
      this.rack_Location = this.qrCodeAgainParam.rack_Location;
      this.material_ID = this.qrCodeAgainParam.material_ID;
      this.supplier_ID = this.qrCodeAgainParam.supplier_ID;
      this.findMaterialName();
      this.search();
    }
    this.inputService.clearDataChangeMenu();
  }
  getTimeNow() {
    const today = this.functionUtility.getToDay();
    this.time_start = today;
    this.time_end = today;
      
  }
  getData() {
    this.checkBoxAll = false;
    this.listParamPrintQrCodeAgain = [];
    this.spinnerService.show();
    this.inputService.qrCodeAgainFilter(this.pagination.currentPage, this.pagination.itemsPerPage, this.qrCodeAgainParam)
      .subscribe((res: PaginatedResult<QrCodeAgainModel[]>) => {
        this.transactionMainListAll = res.result;
        this.transactionMainList = this.transactionMainListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
        this.pagination.totalItems = res.pagination.totalItems;
        this.spinnerService.hide();
        if (this.transactionMainList.length === 0) {
          this.alertifyService.error('No Data!');
        }
      }, error => {
        this.alertifyService.error(error);
      });
  }
  getSupplier() {
    this.packingListService.supplierList().subscribe((res) => {
      this.supplierList = res.map((obj) => {
        return {
          id: obj.supplier_No,
          text: obj.supplier_No + "-" + obj.supplier_Name,
        };
      });
      this.supplierList.unshift({
        id: "All",
        text: "All Supplier",
      });
    });
  }
  changedSupplier(e: any): void {
    this.supplier_ID = e;
  }
  search() {
    this.pagination.currentPage = 1;
    let isSubmit = true;
    if(this.functionUtility.checkEmpty(this.time_start)) {
      if(this.functionUtility.checkEmpty(this.time_end)) {
        this.qrCodeAgainParam = {
          from_Date: '',
          to_Date: '',
          supplier_ID: this.supplier_ID,
          mO_No: this.mO_No,
          rack_Location: this.rack_Location,
          material_ID: this.material_ID
        };
      } else {
        this.alertifyService.error('Please option Date Start!');
        isSubmit = false;
      }
    } else {
      if(this.functionUtility.checkEmpty(this.time_end)) {
        isSubmit = false;
        this.alertifyService.error('Please option Date End!');
      } else {
        let form_date = this.functionUtility.getDateFormat(new Date(this.time_start));
        let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
        this.qrCodeAgainParam = {
          from_Date: form_date,
          to_Date: to_date,
          supplier_ID: this.supplier_ID,
          mO_No: this.mO_No,
          rack_Location: this.rack_Location,
          material_ID: this.material_ID
        };
      }
    }
    if(isSubmit) {
      this.checkBoxAll = false;
      this.inputService.changeCodeAgainParam(this.qrCodeAgainParam);
      this.getData();
    }
  }
  findMaterialName() {
    if (!this.functionUtility.checkEmpty(this.material_ID)) {
      this.inputService.findMaterialName(this.material_ID).subscribe(res => {
        this.material_Name = res.materialName;
      });
    } else {
      this.material_Name = '';
    }
  }
  printQrCodeAgain(model: TransactionMain) {
    this.outputService.changePrintQrCode('1');
    const paramPrintQrCodeAgain = [{
      qrCode_ID: model.qrCode_ID,
      qrCode_Version: model.qrCode_Version,
      mO_Seq: model.mO_Seq
    }];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.transactionMainList = this.transactionMainListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }

  upperCase() {
    this.rack_Location = this.rack_Location.toUpperCase();
  }

  checkPrintLocationInLocalStorage() {
    if (localStorage.getItem('checkPrintLocation') == null) {
      this.checkPrintLocation = 'true';
      localStorage.setItem('checkPrintLocation', 'true');
    }
    else {
      this.checkPrintLocation = localStorage.getItem('checkPrintLocation');
    }
  }

  changeCheckPrintLocation(e) {
    if (e.target.checked) {
      localStorage.setItem('checkPrintLocation', 'true');
    }
    else {
      localStorage.setItem('checkPrintLocation', 'false');
    }
  }

  checkAll(e) {
    if (e.target.checked) {
      this.transactionMainListAll.forEach((element) => {
        element.checked = true;
      });
    }
    else {
      this.transactionMainListAll.forEach((element) => {
        element.checked = false;
      });
    }
  }

  checkEle() {
    let countTransactionMainNotChecked = this.transactionMainListAll.filter(x => x.checked !== true).length;
    if (countTransactionMainNotChecked === 0) {
      this.checkBoxAll = true;
    }
    else {
      this.checkBoxAll = false;
    }
  }

  cancel() {
    this.mO_No = '';
    this.material_ID = '';
    this.material_Name = '';
    this.rack_Location = '';
    this.transactionMainList = [];
    this.inputService.changeCodeAgainParam(this.qrCodeAgainParam);
  }

  print() {
    this.listParamPrintQrCodeAgain = this.transactionMainListAll.filter(item => {
      return item.checked === true;
    }).map(item => {
      return { qrCode_ID: item.qrCode_ID, qrCode_Version: item.qrCode_Version, mO_Seq: item.mO_Seq };
    });

    if (this.listParamPrintQrCodeAgain.length == 0) {
      this.alertifyService.error('Please choose qrcode print!');
      return;
    }
    this.outputService.changePrintQrCode('1');
    this.outputService.changeParamPrintQrCodeAgain(this.listParamPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
  }
}
