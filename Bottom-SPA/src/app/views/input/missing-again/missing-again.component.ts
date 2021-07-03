import { Component, OnInit } from '@angular/core';
import { InputService } from '../../../_core/_services/input.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { Pagination, PaginatedResult } from '../../../_core/_models/pagination';
import { FilterMissingParam } from '../../../_core/_viewmodels/missing-print-search';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { Select2OptionData } from 'ng-select2';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { MissingAgain } from '../../../_core/_models/missing-again';
import { NSP_MISSING_REPORT_DETAIL } from '../../../_core/_models/nsp_missing_report_detail';
import { Router } from '@angular/router';

@Component({
  selector: 'app-missing-again',
  templateUrl: './missing-again.component.html',
  styleUrls: ['./missing-again.component.scss']
})
export class MissingAgainComponent implements OnInit {
  missingAgainList: MissingAgain[] = [];
  missingAgainListAll: MissingAgain[] = [];
  mO_No: string;
  material_ID: string;
  material_Name: string;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 5,
    totalItems: 0,
    totalPages: 0
  };
  missingParam: FilterMissingParam;
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
  bsConfig = Object.assign(
    {},
    {
      containerClass: "theme-blue",
      isAnimated: true,
      dateInputFormat: "YYYY/MM/DD",
    }
  );
  fromTime: Date = new Date;
  toTime: Date = new Date;
  supplierList: Array<Select2OptionData>;
  supplier_ID: string = 'All';
  missingReportDetail: NSP_MISSING_REPORT_DETAIL[];
  checkboxAll: boolean = false;
  downloaded: string ='All';

  constructor(private inputService: InputService,
    private functionUtility: FunctionUtility,
    private spinnerService: NgxSpinnerService,
    private alertifyService: AlertifyService,
    private router: Router,
    private packingListService: PackingListService) { }

  ngOnInit() {
    this.getListSupplier();
    this.inputService.currentMissingParam.subscribe(res => this.missingParam = res);
    if (this.missingParam !== undefined && this.missingParam !== null) {
      this.mO_No = this.missingParam.mO_No;
      this.material_ID = this.missingParam.material_ID;
      this.fromTime = this.missingParam.fromTime;
      this.toTime = this.missingParam.toTime;
      this.supplier_ID = this.missingParam.supplier_ID;
      this.downloaded = this.missingParam.downloaded
      if (this.material_ID !== undefined) {
        this.findMaterialName();
      }
      this.search();
    }
    this.inputService.clearDataChangeMenu();
  }
  
  getData() {
    this.checkboxAll = false;
    this.spinnerService.show();
    let param = {
      mO_No: this.mO_No,
      material_ID: this.material_ID,
      fromTime: this.fromTime == null ? '' : this.functionUtility.getDateFormat(this.fromTime),
      toTime: this.toTime == null ? '' : this.functionUtility.getDateFormat(this.toTime),
      supplier_ID: this.supplier_ID == 'All' ? '' : this.supplier_ID,
      downloaded:this.downloaded
    }
    this.inputService.missingPrintFilter(this.pagination.currentPage, this.pagination.itemsPerPage, param)
      .subscribe((res: PaginatedResult<MissingAgain[]>) => {
        this.missingAgainListAll = res.result;
        this.pagination = res.pagination;
        this.missingAgainList = this.missingAgainListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
        this.spinnerService.hide();
        if (this.missingAgainList.length === 0) {
          this.alertifyService.error('No Data!');
        }
      }, error => {
        this.alertifyService.error(error);
      });
  }

  findMaterialName() {
    if (!(this.functionUtility.checkEmpty(this.material_ID))) {
      this.inputService.findMaterialName(this.material_ID).subscribe(res => {
        this.material_Name = res.materialName;
      });
    } else {
      this.material_Name = '';
    }
  }

  search() {
    if(this.fromTime > this.toTime) {
      this.alertifyService.error('Please option Start date less than end date');
      return;
    }
    this.pagination.currentPage = 1;
    this.missingParam = {
      mO_No: this.mO_No,
      material_ID: this.material_ID,
      fromTime: this.fromTime,
      toTime: this.toTime,
      supplier_ID: this.supplier_ID,
      downloaded:this.downloaded
    }
    this.inputService.changeMissingParam(this.missingParam);
    this.getData();
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.missingAgainList = this.missingAgainListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }
  printMissing(item: MissingAgain) {
    this.inputService.missingPrintSource.next(item);
    this.router.navigate(['/input/missing-print']);
  }
  getListSupplier() {
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

  // Export Excel Missing Report Detail
  exportExcel() {
    const listMissingNo = this.missingAgainListAll.filter(item => {
      return item.checked == true;
    }).map(item => {
      return {
        missingNo: item.missing_No,
        batch: item.mO_Seq
      }
    })
    if (listMissingNo.length == 0) {
      return this.alertifyService.error('Please choose item to export excel');
    }
    let param = {
      mO_No: this.mO_No == null ? '' : this.mO_No,
      material_ID: this.material_ID == null ? '' : this.material_ID,
      fromTime: this.fromTime == null ? '' : this.functionUtility.getDateFormat(this.fromTime),
      toTime: this.toTime == null ? '' : this.functionUtility.getDateFormat(this.toTime),
      supplier_ID: this.supplier_ID == 'All' ? '' : this.supplier_ID,
      listMissingNo: listMissingNo
    }
    this.inputService.exportExcelMissingReportDetail(param);
  }

  checkAll(e) {
    if (e.target.checked) {
      this.missingAgainListAll.forEach(element => {
        element.checked = true;
      });
    }
    else {
      this.missingAgainListAll.forEach(element => {
        element.checked = false;
      });
    }
  }

  checkElement() {
    let countMisingCheckBox = this.missingAgainListAll.filter(x => x.checked !== true).length;
    if (countMisingCheckBox === 0) {
      this.checkboxAll = true;
    } else {
      this.checkboxAll = false;
    }
  }
}
