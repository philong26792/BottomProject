import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { SettingMailSupllier } from '../../../_core/_models/setting-mail-supplier';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { InputService } from '../../../_core/_services/input.service';
import { SettingMailService } from '../../../_core/_services/setting-mail.service';
import { commonPerFactory } from '../../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-list-mail',
  templateUrl: './list-mail.component.html',
  styleUrls: ['./list-mail.component.scss']
})
export class ListMailComponent implements OnInit {
  listFactory: Array<Select2OptionData> = [{ id: commonPerFactory.defaultRackFactory, text: commonPerFactory.defaultRackFactory }];
  factory: string = commonPerFactory.defaultRackFactory;
  listSupplierNo: Array<Select2OptionData>;
  dataSupplier: any;
  dataSubcon: any;

  supplierNo: string = "all";
  listMail: SettingMailSupllier[] = [];
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  }

  constructor(private _service: SettingMailService, 
              private router: Router,
              private spinnerService: NgxSpinnerService,
              private inputService: InputService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.getDataSupplierNo();
    this.getDataSubcon();
    this.getListSettingMail();
    this.inputService.clearDataChangeMenu();
  }

  //get data supplierNo
  getDataSupplierNo() {
    this._service.getAllSupllierNo().subscribe(res => {
      this.dataSupplier = res;
      this.listSupplierNo = res.map(item => {
        return { id: item.t3_Supplier, text: item.t3_Supplier };
      });
      this.listSupplierNo.unshift({ id: "all", text: "All" });
      this.listSupplierNo.push({ id: 'ZZZZ', text: 'ZZZZ' });
    })
  }

  getListSettingMail(currentPage = 1) {
    this.spinnerService.show();
    this._service.search(currentPage, commonPerFactory.defaultRackFactory, this.supplierNo).subscribe(res => {
      this.listMail = res.result;
      this.pagination = res.pagination;
      this.spinnerService.hide();
    })
  }

  getDataSubcon() {
    this._service.getAllSubcon().subscribe(res => {
      this.dataSubcon = res;
    })
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.getListSettingMail(this.pagination.currentPage);
  }

  addAndEdit(item?: SettingMailSupllier, check?: boolean) {
    this.listSupplierNo.shift();
    var data = {
      item,
      check,
      dataSupplier: this.dataSupplier,
      dataSubcon: this.dataSubcon
    };
    this._service.getDataAddAndEdit(data);
    this.router.navigate(['/rack/setting-mail/add-mail'])
  }

  delete(item: SettingMailSupllier) {
    this.alertify.confirm('Delete SettingMail', 'Are you sure you want to delete this setting mail supplier  ?', () => {
      this._service.deleteSettingMail(item).subscribe(() => {
        this.alertify.success("Delete setting mail supplier success")
        this.supplierNo = 'all';
        this.getListSettingMail();
      }, error => {
        this.alertify.error("Delete setting mail error");
      })
    });
  }
}
