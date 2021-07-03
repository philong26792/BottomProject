import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { MaterialService } from '../../../_core/_services/material.service';
import { Router } from '@angular/router';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Pagination, PaginatedResult } from '../../../_core/_models/pagination';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { MaterialModel } from '../../../_core/_viewmodels/material-model';
import * as _ from 'lodash'; 
import { MaterialSearch } from '../../../_core/_viewmodels/material-search';
import { InputService } from '../../../_core/_services/input.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { NgxSpinnerService } from 'ngx-spinner';
import { Select2OptionData } from "ng-select2";
@Component({
  selector: 'app-receipt-main',
  templateUrl: './receipt-main.component.html',
  styleUrls: ['./receipt-main.component.scss']
})
export class ReceiptMainComponent implements OnInit {
  disable = false;
  bsConfig: Partial<BsDatepickerConfig>;
  pagination: Pagination;
  time_start: string;
  time_end: string;
  mO_No: string;
  supplier_ID: string;
  public supplierList: Array<Select2OptionData>;
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%"
  };
  materialSearch: MaterialSearch;
  materialLists: MaterialModel[] = [];
  materialAll: MaterialModel[] = [];
  status: string = 'all';
  dMONo: string = '';
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

  constructor(private materialService: MaterialService,
              private packingListService: PackingListService,
              private inputService: InputService,
              private router: Router,
              private spinnerService: NgxSpinnerService,
              private alertifyService: AlertifyService,
              private functionUtility: FunctionUtility) { }

  ngOnInit() {
    this.getSupplier();
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
    this.materialService.currentMaterialSearch.subscribe(res => this.materialSearch = res);
    if (this.materialSearch !== undefined && this.materialSearch !== null) {
      this.mO_No = this.materialSearch.mO_No;
      this.supplier_ID = this.materialSearch.supplier_ID;
      this.status = this.materialSearch.status;
      if(this.materialSearch.from_Date === null) {
        this.time_start = '';
      } else {
        this.time_start = this.materialSearch.from_Date;
      }
      if(this.materialSearch.to_Date === null) {
        this.time_end = '';
      } else {
        this.time_end = this.materialSearch.to_Date;
      }

      if(this.materialSearch.from_Date === undefined && this.materialSearch.mO_No === undefined) {

      } else {
        this.search();
      }
    }
    this.inputService.clearDataChangeMenu();
  }
  getData() {
    this.spinnerService.show();
        this.materialService.search(this.pagination.currentPage , this.pagination.itemsPerPage, this.materialSearch)
          .subscribe((res: PaginatedResult<MaterialModel[]>) => {
            this.materialAll = res.result;
            this.materialLists = this.materialAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
            this.pagination = res.pagination;
            this.spinnerService.hide();
            if(this.materialLists.length === 0) {
              this.alertifyService.error('No Data!');
            }
          }, error => {
            this.alertifyService.error(error);
        });
  }
  getSupplier() {
      this.packingListService.supplierList().subscribe(res => {
        this.supplierList = res.map(obj => {
          let supplier = {
            id:obj.supplier_No,
            text: obj.supplier_No + '-' + obj.supplier_Name
          }
          return supplier;
        });
        this.supplierList.unshift({
          id:"All",
          text: "All Supplier"
        });
      });
  }
  changedSupplier(e: any): void {
    this.supplier_ID = e;
  }
  search() {
      let checkSearch = true;
      if(this.functionUtility.checkEmpty(this.time_start)) {
        if(this.functionUtility.checkEmpty(this.time_end)) {
          if(this.functionUtility.checkEmpty(this.mO_No)) {
            checkSearch = false;
            this.alertifyService.error('Please option Expected Delivery Date Or Plan No!');
          } else {
            this.materialSearch = {
              supplier_ID: this.supplier_ID,
              mO_No: this.mO_No,
              from_Date: null,
              to_Date: null,
              status: this.status
            };
          }
        } else {
          checkSearch = false;
          this.alertifyService.error('Please option Start Time')
        }
      } else {
        if(this.functionUtility.checkEmpty(this.time_end)) {
          checkSearch = false;
          this.alertifyService.error('Please option time end!');
        } else {
          let diff =  Math.floor(( Date.parse(this.time_end) - Date.parse(this.time_start) ) / 86400000);
          if(diff > 7) {
            checkSearch = false;
            this.alertifyService.error("Please enter a period of 7 days!");
          } else {
            checkSearch = true;
            let form_date = this.functionUtility.getDateFormat(new Date(this.time_start));
            let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
            this.materialSearch = {
              supplier_ID: this.supplier_ID,
              mO_No: this.mO_No,
              from_Date: form_date,
              to_Date: to_date,
              status: this.status
            };
          }
        }
      }
      if(checkSearch) {
        this.materialService.getDMONo(this.mO_No).subscribe(res => {
          if(res !== null) {
            this.dMONo = res.dmO_No.trim();
          } else {
            this.dMONo = '';
          }
        });
        this.pagination.currentPage = 1;
        this.materialService.changeMaterialSearch(this.materialSearch);
        this.getData();
      }
  }
  changePageAdd(materialModel) {
    this.materialService.changeMaterialModel(materialModel);
    this.router.navigate(['receipt/record']);
  }
  changeStatus(materialModel) {
    this.alertifyService.confirm('Close Purchase No', 'Are you sure Close ?', () => {
      this.materialService.closePurchase(materialModel).subscribe(res => {
        this.materialLists.forEach(item => {
          if (_.isEqual(item,materialModel)) {
            item.status = 'Y';
            return;
          }
        });
        this.alertifyService.success('Close successed!');
      }, error => {
        this.alertifyService.error(error);
      });
    });
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.materialLists = this.materialAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
    // this.getData();
  }
}
