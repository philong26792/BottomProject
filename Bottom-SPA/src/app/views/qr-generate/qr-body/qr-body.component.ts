import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QrcodeMainService } from '../../../_core/_services/qrcode-main.service';
import { QRCodeMainModel } from '../../../_core/_viewmodels/qrcode-main-model';
import { Pagination, PaginatedResult } from '../../../_core/_models/pagination';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { PackingListDetailModel } from '../../../_core/_viewmodels/packing-list-detail-model';
import { PackingPrintAll } from '../../../_core/_viewmodels/packing-print-all';
import { QRCodeMainSearch } from '../../../_core/_viewmodels/qrcode-main-search';
import { InputService } from '../../../_core/_services/input.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { NgxSpinnerService } from 'ngx-spinner';
import { MaterialFormService } from '../../../_core/_services/material-form.service';

@Component({
  selector: 'app-qr-body',
  templateUrl: './qr-body.component.html',
  styleUrls: ['./qr-body.component.scss']
})
export class QrBodyComponent implements OnInit {
  suggested =  true;
  pagination: Pagination;
  bsConfig: Partial<BsDatepickerConfig>;
  listQrCodeMainModel: QRCodeMainModel[] = [];
  listQrCodeMainModelAll: QRCodeMainModel[] = [];
  packingListDetailAll: PackingListDetailModel[][] = [];
  time_start: string;
  time_end: string;
  mO_No: string;
  qrCodeMainSearch: QRCodeMainSearch;
  packingPrintAll: PackingPrintAll[] = [];
  checkboxAll: boolean = false;
  checkSearch = false;
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
  constructor(private router: Router,
              private qrCodeMainService: QrcodeMainService,
              private inputService: InputService,
              private materialFormService: MaterialFormService,
              private alertifyService: AlertifyService,
              private functionUtility: FunctionUtility,
              private spinnerService: NgxSpinnerService) { }

  ngOnInit() {
      let suggested = localStorage.getItem('suggested');
      if (suggested == null) {
        localStorage.setItem('suggested', '1');
        suggested = '1';
      }
      if(suggested.toString() === '1') {
        this.suggested = true;
      } else {
        this.suggested = false;
      }

      this.pagination = {
        currentPage: 1,
        itemsPerPage: 3,
        totalItems: 0,
        totalPages: 0
      };
      this.bsConfig = Object.assign(
        {},
        {
          containerClass: 'theme-blue',
          isAnimated: true,
          dateInputFormat: 'YYYY/MM/DD',
        }
      );
      this.qrCodeMainService.currentQrCodeMainSearch.subscribe(res => this.qrCodeMainSearch = res);
      if (this.qrCodeMainSearch === null) {
        this.getDataLoadPage();
      } else {
        this.mO_No = this.qrCodeMainSearch.mO_No;
        this.time_start = this.qrCodeMainSearch.from_Date;
        this.time_end = this.qrCodeMainSearch.to_Date;
        this.search();
      }
      this.inputService.clearDataChangeMenu();
  }
  getData() {
    this.spinnerService.show();
    this.qrCodeMainService.search(this.pagination.currentPage , this.pagination.itemsPerPage, this.qrCodeMainSearch)
    .subscribe((res: PaginatedResult<QRCodeMainModel[]>) => {
      this.listQrCodeMainModelAll = res.result.map(obj => {
        obj.checkInput = false;
        return obj;
      });
      this.listQrCodeMainModel = this.listQrCodeMainModelAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
      this.pagination = res.pagination;
      if(this.listQrCodeMainModel.length === 0) {
        this.alertifyService.error('No Data!');
      }
      this.spinnerService.hide();
    }, error => {
      this.alertifyService.error(error);
    });
  }
  getDataLoadPage() {
    const timeNow = this.functionUtility.getToDay();
    this.time_start = timeNow;
    this.time_end = timeNow;
    let form_date = this.functionUtility.getDateFormat(new Date(timeNow));
    let to_date = this.functionUtility.getDateFormat(new Date(timeNow));
    if (this.mO_No === undefined) {
      this.mO_No = '';
    }
    this.qrCodeMainSearch = {
      mO_No: '',
      from_Date: form_date,
      to_Date: to_date
    };
    this.qrCodeMainService.changeQrCodeMainSearch(this.qrCodeMainSearch);
    this.getData();
  }
  search() {
    this.pagination.currentPage = 1;
        let checkSearch = true;
        if (this.time_start !== null) {
          if (this.time_end === null) {
            this.alertifyService.error('Please option time end!');
            checkSearch = false;
          }
        }
        if(checkSearch) {
          if (this.time_start === null) {
            this.qrCodeMainSearch = {mO_No: this.mO_No,from_Date: null,to_Date: null};
          } else {
            let form_date = this.functionUtility.getDateFormat(new Date(this.time_start));
            let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
            this.qrCodeMainSearch = {mO_No: this.mO_No,from_Date: form_date,to_Date: to_date};
          }
          if (this.mO_No === undefined) {
            this.mO_No = null;
          }
          this.qrCodeMainService.changeQrCodeMainSearch(this.qrCodeMainSearch);
          this.getData();
      }
  }
  print(qrCodeMain: QRCodeMainModel) {
      this.spinnerService.show();
      let param = [];
      let qrCodeIDItem = {
        qrCode_ID: qrCodeMain.qrCode_ID,
        qrCode_Version: qrCodeMain.qrCode_Version,
        mO_No: qrCodeMain.mO_No,
        mO_Seq: qrCodeMain.mO_Seq
      };
      param.push(qrCodeIDItem);
      this.materialFormService.printMaterialForm(param).subscribe(res => {
        this.spinnerService.hide();
        this.packingPrintAll = res;
        this.packingPrintAll = this.packingPrintAll.map(obj => {
          if(obj.suggestedReturn1.length > 3) {
            obj.suggestedReturn1.length = 3;
          }
          return obj;
        });
        this.materialFormService.changePackingPrint(this.packingPrintAll);
        this.router.navigate(['/qr/print']);
      })
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.listQrCodeMainModel = this.listQrCodeMainModelAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }
  checkAll(e) {
    if (e.target.checked) {
      this.listQrCodeMainModelAll.map(obj => {
        obj.checkInput = true;
        return obj;
      });
    } else {
      this.listQrCodeMainModelAll.map(obj => {
        obj.checkInput = false;
        return obj;
      });
    }
  }
  onCheckboxChange() {
    let countQrCodeMainNotChecked = this.listQrCodeMainModelAll.filter(x => x.checkInput != true).length;
    if(countQrCodeMainNotChecked === 0) {
      this.checkboxAll = true;
    } else {
      this.checkboxAll = false;
    }
  }
  printAll() {
    let listQrCodeMainChecked = this.listQrCodeMainModelAll.filter(x => x.checkInput === true);
    if (listQrCodeMainChecked.length !== 0) {
      let param = [];
      this.spinnerService.show();
      listQrCodeMainChecked.forEach(item => {
        let paramItem = {
          qrCode_ID: item.qrCode_ID,
          qrCode_Version: item.qrCode_Version,
          mO_No: item.mO_No,
          mO_Seq: item.mO_Seq
        };
        param.push(paramItem);
      });
      this.materialFormService.printMaterialForm(param).subscribe(res => {
        this.spinnerService.hide();
        this.packingPrintAll = res;
        
        this.packingPrintAll = this.packingPrintAll.map(obj => {
          if(obj.suggestedReturn1.length > 3) {
            obj.suggestedReturn1.length = 3;
          }
          return obj;
        });
        this.materialFormService.changePackingPrint(this.packingPrintAll);
        this.router.navigate(['/qr/print']);
      });
    } else {
      this.alertifyService.error('Please check in checkbox!');
    }
  }
  suggestedClick() {
    let suggested;
    // không tick chọn
    if(this.suggested) {
      suggested = '0'
    } else {
      // tick chọn
      suggested = '1'
    }
    localStorage.setItem('suggested', suggested);
  }
  cancel() {
    this.mO_No = '';
    this.listQrCodeMainModelAll.length = 0;
    this.listQrCodeMainModel.length = 0;
    this.checkboxAll = false;
  }
}
