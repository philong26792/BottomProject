import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { TransferFormGenerate } from '../../../_core/_models/transfer-form-generate';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { SettingMailService } from '../../../_core/_services/setting-mail.service';
import { TransferFormService } from '../../../_core/_services/transfer-form.service';

@Component({
  selector: 'app-print',
  templateUrl: './print.component.html',
  styleUrls: ['./print.component.scss']
})
export class PrintComponent implements OnInit {
  fromTime: Date = new Date();
  toTime: Date = new Date();
  bsConfig: Partial<BsDatepickerConfig>;
  release: string = 'N';
  planNo: string = '';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };
  transferFormPrint: TransferFormGenerate[] = [];
  transferFormPrintAll: TransferFormGenerate[] = [];
  transferFormPrintBefore: TransferFormGenerate[] = [];
  checkboxAll: boolean = false;
  isRelease: boolean;
  t3Suppliers: Array<Select2OptionData> = [];
  t3Supplier: string = 'all';
  t2Suppliers: Array<Select2OptionData> = [];
  t2Supplier: string = 'all';
  checkSearch: boolean = false;

  constructor(private transferFormService: TransferFormService,
    private spinnerService: NgxSpinnerService,
    private alertifyService: AlertifyService,
    private router: Router,
    private settingMailService: SettingMailService,
    private packingListService: PackingListService) { }
  ngOnDestroy(): void {
    const paramSearch = {
      fromTime: this.fromTime,
      toTime: this.toTime,
      planNo: this.planNo,
      release: this.release,
      currentPage: this.pagination.currentPage,
      t3Supplier: this.t3Supplier
    };
    this.transferFormService.changeparamSearchTransferFormPrint(paramSearch);
  }

  ngOnInit() {
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: 'theme-blue',
        isAnimated: true,
        dateInputFormat: 'YYYY/MM/DD',
      }
    );
    this.transferFormService.currentParamSearchTransferFormPrint.subscribe(res => {
      this.fromTime = res.fromTime;
      this.toTime = res.toTime;
      this.release = res.release;
      this.planNo = res.planNo;
      this.pagination.currentPage = res.currentPage;
      this.t3Supplier = res.t3Supplier;
    });
    this.getT3Supplier();
    this.getT2Supplier();
    this.transferFormService.currentCheckSearch.subscribe(res => {
      if(res) {
        this.getData();
      }
    });
  }

  search() {
    this.checkSearch = true;
    this.pagination.currentPage = 1;
    this.getData();
  }

  sendEmail() {
    this.checkSearch = false;
    const listSendEmailTransferForm = this.transferFormPrintAll.filter(item => {
      return item.checked === true;
    });

    if (listSendEmailTransferForm.length === 0) {
      this.alertifyService.error('Please chosing transferring form to send email!');
      return;
    }

    const listSenEmailTransferFormNotHaveEmail = listSendEmailTransferForm.filter(item => {
      return item.email == null;
    });
    if (listSenEmailTransferFormNotHaveEmail.length > 0) {
      const listT3Supplier = listSenEmailTransferFormNotHaveEmail.map(item => {
        return item.t3_Supplier;
      });
      this.alertifyService.error('Please setting ' + listT3Supplier + ' in the Setting_Supplier table');
      return;
    }

    this.transferFormService.sendEmail(listSendEmailTransferForm).subscribe();
    this.spinnerService.show();
    this.transferFormPrintBefore = JSON.parse(JSON.stringify(this.transferFormPrintAll));
    setTimeout(() => {
      this.alertifyService.success('Send Email Success');
      this.getData();
      this.spinnerService.hide();
    }, 1000);
  }

  printTransaction() {
    const listPrintTransferForm = this.transferFormPrintAll.filter(item => {
      return item.checked === true;
    });

    if (listPrintTransferForm.length === 0) {
      this.alertifyService.error('Please chosing transferring form to print!');
      return;
    }

    this.transferFormService.changeTransferFormPrintDetail(listPrintTransferForm);
    this.router.navigateByUrl('/input/transfer-form/print-form');
  }

  releaseTransferForm() {
    const listReleaseTransferForm = this.transferFormPrintAll.filter(item => {
      return item.checked === true;
    });

    if (listReleaseTransferForm.length === 0) {
      this.alertifyService.error('Please chosing transferring form to release!');
      return;
    }
    this.spinnerService.show();
    this.transferFormService.releaseTransferForm(listReleaseTransferForm).subscribe(() => {
      this.alertifyService.success('Release Success!');
      this.getData();
    }, error => {
      this.alertifyService.error('Release Fail!');
      console.log(error);
      this.spinnerService.hide();
    });
  }

  getData() {
    this.checkboxAll = false;
    this.spinnerService.show();
    this.transferFormService.getTransferFormPrint(this.fromTime, this.toTime, this.planNo,
      this.release, this.t2Supplier, this.t3Supplier, this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe(res => {
        this.pagination = res.pagination;
        this.transferFormPrintAll = res.result;
        this.transferFormPrint = this.transferFormPrintAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
        if (this.transferFormPrint.length > 0) {
          this.isRelease = this.transferFormPrint[0].is_Release === 'Y' ? true : false;
        }
        if(!this.checkSearch) {
          if(this.transferFormPrintBefore.length > 0) {
            this.transferFormPrintAll.forEach(item => {
              let transferFormModel = this.transferFormPrintBefore.filter(x => x.collect_Trans_No.trim() === item.collect_Trans_No.trim())[0];
              item.checked = transferFormModel.checked;
            });
          }
        }
        this.spinnerService.hide();
      }, error => {
        this.alertifyService.error('Error Server!');
        console.log(error);
        this.spinnerService.hide();
      });
  }

  checkAll(e) {
    if (e.target.checked) {
      this.transferFormPrintAll.forEach(element => {
        element.checked = true;
      });
    }
    else {
      this.transferFormPrintAll.forEach(element => {
        element.checked = false;
      });
    }
  }

  checkElement() {
    let countTransferFormNotCheck = this.transferFormPrintAll.filter(x => x.checked !== true).length;
    if (countTransferFormNotCheck === 0) {
      this.checkboxAll = true;
    } else {
      this.checkboxAll = false;
    }
  }
  pageChanged(e) {
    this.pagination.currentPage = e.page;
    this.transferFormPrint = this.transferFormPrintAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }

  getT3Supplier() {
    this.settingMailService.getAllSupllierNo().subscribe(res => {
      this.t3Suppliers = res.map(item => {
        return { id: item.t3_Supplier, text: item.t3_Supplier + ' - ' + item.t3_Supplier_Name };
      });
      this.t3Suppliers.unshift({ id: 'all', text: 'ALL' });
    });
  }

  getT2Supplier() {
    this.packingListService.supplierList().subscribe(res => {
      this.t2Suppliers = res.map(item => {
        return { id: item.supplier_No, text: item.supplier_No + ' - ' + item.supplier_Name };
      });
      this.t2Suppliers.unshift({ id: 'all', text: 'ALL' });
    });
  }
}
