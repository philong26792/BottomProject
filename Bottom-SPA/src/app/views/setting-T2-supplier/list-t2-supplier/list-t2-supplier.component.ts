import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { SettingT2Delivery, SettingT2Param } from '../../../_core/_models/setting-t2-supplier';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { SettingT2SupplierService } from '../../../_core/_services/setting-t2-supplier.service';
import { commonPerFactory } from '../../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-list-t2-supplier',
  templateUrl: './list-t2-supplier.component.html',
  styleUrls: ['./list-t2-supplier.component.css']
})
export class ListT2SupplierComponent implements OnInit {
  listFactory: Array<Select2OptionData> = [{ id: commonPerFactory.defaultRackFactory, text: commonPerFactory.defaultRackFactory == 'C' ? 'SHC' : commonPerFactory.defaultRackFactory }];
  factory: string = commonPerFactory.defaultRackFactory;
  listT2supplier: SettingT2Delivery[] = [];
  listReason: Array<Select2OptionData> = [];

  supplierIDList: Array<Select2OptionData>;
  constructor(
    private settingT2service: SettingT2SupplierService,
    private packingListService: PackingListService,
    private router: Router,
    private alert: AlertifyService,
    private spinner: NgxSpinnerService) { }

  params: SettingT2Param = {
    factory_id: this.factory,
    reason_code: '',
    supplier_id: '',
    input_delivery: ''
  };
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  }
  optionsSelectSupplierID = {
    placeholder: "Select Supplier...",
    allowClear: true,
    width: "100%",
  };
  optionsSelectReasonCode = {
    placeholder: "Select Reason...",
    allowClear: true,
    width: "100%",
  }
  ngOnInit(): void {
    this.getSupplierList();
    this.getReason();
    this.search();
  }

  search() {
    this.spinner.show();
    this.pagination.currentPage = 1;
    this.getData();
  }

  getData() {
    this.settingT2service.getAll(this.pagination, this.params).subscribe(res => {
      this.listT2supplier = res.result;
      this.pagination = res.pagination;
      this.spinner.hide();
    }, err => {
      this.alert.error("Have error");
      this.spinner.hide();
    });
  }

  getSupplierList() {
    this.packingListService.supplierList().subscribe(res => {
      this.supplierIDList = res.map(item => {
        return {id: item.supplier_No, text: `${item.supplier_No}-${item.supplier_Name}`};
      });
      this.supplierIDList.unshift({ id: ' ', text: 'All Supplier' });
    });
  }

  getReason() {
    this.settingT2service.getAllReason().subscribe(res => {
      this.listReason = res.map(item => {
        return { id: item.reason_Code, text: item.reason_Code + '-' + item.reason_Ename }
      });
      this.listReason.unshift({ id: ' ', text: 'All' });
    });
  }
  AddorEdit(item?: SettingT2Delivery, check?: boolean) {
    var data = { item, check };
    this.settingT2service.dataAddorEdit.next(data);
    this.router.navigateByUrl("/rack/setting-t2-supplier/add-t2-supplier");
  }
  delete(item: SettingT2Delivery) {
    this.alert.confirm('Delete T2 Delivery', 'Are you sure you want to delete this setting T2 supplier ?', () => {
      this.settingT2service.delete(item).subscribe(() => {
        this.alert.success("Delete setting T2 supplier success");
        this.search();
      }, err => this.alert.error("Delete setting T2 supplier error"))
    });
  }
  changeFactory(e: any): void {
    this.params.factory_id = e;
    this.factory = e;
    this.search();
  }

  changeReason(e: any): void {
    this.params.reason_code = e;
    this.search();
  }

  changedSupplierID(e: any): void {
    this.params.supplier_id = e;
    this.search();
  }

  changeInput(e: any): void {
    this.params.input_delivery = e.target.value;
    this.search();
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.getData();
  }

}
