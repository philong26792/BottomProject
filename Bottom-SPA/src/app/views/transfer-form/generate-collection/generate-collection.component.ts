import { Component, OnInit } from '@angular/core';
import { Select2OptionData } from 'ng-select2';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { TransferFormGenerate } from '../../../_core/_models/transfer-form-generate';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { InputService } from '../../../_core/_services/input.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { SettingMailService } from '../../../_core/_services/setting-mail.service';
import { TransferFormService } from '../../../_core/_services/transfer-form.service';

@Component({
  selector: 'app-generate-collection',
  templateUrl: './generate-collection.component.html',
  styleUrls: ['./generate-collection.component.scss']
})
export class GenerateCollectionComponent implements OnInit {
  fromTime: Date = new Date();
  toTime: Date = new Date();
  bsConfig: Partial<BsDatepickerConfig>;
  subcont: string = 'Y';
  planNo: string = '';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };
  transferFormGenerates: TransferFormGenerate[] = [];
  transferFormGeneratesAll: TransferFormGenerate[] = [];
  checkboxAll: boolean = false;
  t3Suppliers: Array<Select2OptionData> = [];
  t3Supplier: string = 'all';
  t2Suppliers: Array<Select2OptionData> = [];
  t2Supplier: string = 'all';

  constructor(private transferFormService: TransferFormService,
              private inputService: InputService,
              private spinnerService: NgxSpinnerService,
              private alertifyService: AlertifyService,
              private settingMailService: SettingMailService,
              private packingListService: PackingListService) { }

  ngOnInit() {
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: 'theme-blue',
        isAnimated: true,
        dateInputFormat: 'YYYY/MM/DD',
      }
    );
    this.getT2Supplier();
    this.getT3Supplier();
    this.getData();
    this.inputService.clearDataChangeMenu();
  }

  search() {
    this.pagination.currentPage = 1;
    this.getData();
  }

  generateTransaction() {
    const listGenerateTransferForm = this.transferFormGeneratesAll.filter(item => {
      return item.checked === true;
    });
    if (listGenerateTransferForm.length === 0) {
      this.alertifyService.error('Please choose Generate Transaction!');
      return;
    }
    this.spinnerService.show();
    this.transferFormService.generateTransferForm(listGenerateTransferForm).subscribe(() => {
      this.alertifyService.success('Generate Transaction Success!');
      this.getData();
    }, error => {
      this.alertifyService.error('Generate Transaction Fail!');
      console.log(error);
      this.spinnerService.hide();
    });
  }

  getData() {
    this.checkboxAll = false;
    this.spinnerService.show();
    this.transferFormService.getTransferFormGenerate(this.fromTime, this.toTime, this.planNo,
      this.subcont, this.t2Supplier, this.t3Supplier, this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe(res => {
        this.pagination = res.pagination;
        this.transferFormGeneratesAll = res.result;
        this.transferFormGenerates = this.transferFormGeneratesAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
        this.spinnerService.hide();
      }, error => {
        this.alertifyService.error('Error Server!');
        console.log(error);
        this.spinnerService.hide();
      });
  }

  checkAll(e) {
    if (e.target.checked) {
      this.transferFormGeneratesAll.forEach(element => {
        element.checked = true;
      });
    }
    else {
      this.transferFormGeneratesAll.forEach(element => {
        element.checked = false;
      });
    }
  }

  checkElement() {
    let countTransferFormNotCheck = this.transferFormGeneratesAll.filter(x => x.checked !== true).length;
    if(countTransferFormNotCheck === 0){
      this.checkboxAll = true;
    } else {
      this.checkboxAll = false;
    }
  }
  pageChanged(e) {
    this.pagination.currentPage = e.page;
    this.transferFormGenerates = this.transferFormGeneratesAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
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
