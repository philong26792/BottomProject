import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { TransferM } from '../../../_core/_models/transferM';
import { TransferService } from '../../../_core/_services/transfer.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { Pagination } from '../../../_core/_models/pagination';
import { NgxSpinnerService } from 'ngx-spinner';
import { InputService } from '../../../_core/_services/input.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { Select2OptionData } from 'ng-select2';

@Component({
  selector: "app-transfer-history",
  templateUrl: "./transfer-history.component.html",
  styleUrls: ["./transfer-history.component.scss"],
})
export class TransferHistoryComponent implements OnInit {
  bsConfig: Partial<BsDatepickerConfig>;
  public supplierList: Array<Select2OptionData>;
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%",
  };
  disable = false;
  supplier_ID: string = "";
  fromDate: string;
  toDate: string;
  transfers: TransferM[] = [];
  printArray: TransferM[] = [];
  status: string = "";
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  mO_No: string = "";
  material_ID: string;
  material_Name: string;
  autoLoad: boolean = false;
  intervalCurrent: any;
  constructor(
    private transferService: TransferService,
    private packingListService: PackingListService,
    private spinnerService: NgxSpinnerService,
    private alertify: AlertifyService,
    private functionUtility: FunctionUtility,
    private inputService: InputService
  ) {}

  ngOnInit() {
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: "theme-blue",
        isAnimated: true,
        dateInputFormat: "YYYY/MM/DD",
      }
    );
    this.transferService.currentParamSearch.subscribe((res) => {
      this.fromDate = res.fromDate;
      this.toDate = res.toDate;
      this.status = res.status;
      this.mO_No = res.moNo;
      this.material_ID = res.materialId;
    });
    this.getData();
    this.getListSupplier();
    this.inputService.clearDataChangeMenu();
  }

  getData() {
    let isGet = true;
    let transferHistoryParam;
    if (this.functionUtility.checkEmpty(this.fromDate)) {
      if (this.functionUtility.checkEmpty(this.toDate)) {
        transferHistoryParam = {
          toDate: "",
          fromDate: "",
          status: this.status,
          moNo: this.mO_No,
          materialId: this.material_ID,
          supplier_ID: this.supplier_ID
        };
      } else {
        isGet = false;
        this.alertify.error("Please enter start time!");
      }
    } else {
      if (this.functionUtility.checkEmpty(this.toDate)) {
        isGet = false;
        this.alertify.error("Please enter end time!");
      } else {
        const t1 = this.functionUtility.getDateFormat(new Date(this.fromDate));
        const t2 = this.functionUtility.getDateFormat(new Date(this.toDate));
        transferHistoryParam = {
          toDate: t2,
          fromDate: t1,
          status: this.status,
          moNo: this.mO_No,
          materialId: this.material_ID,
          supplier_ID: this.supplier_ID
        };
      }
    }

    if (isGet) {
      this.spinnerService.show();
      this.transferService.changeParamSearch(transferHistoryParam);
      this.transferService
        .search(
          this.pagination.currentPage,
          this.pagination.itemsPerPage,
          transferHistoryParam
        )
        .subscribe((res) => {
          this.transfers = res.result;
          this.pagination = res.pagination;
          this.spinnerService.hide();
        });
    }
  }
  autoNextPage() {
    if(this.autoLoad === true) {
      this.intervalCurrent = setInterval(() => {
        if(this.pagination.currentPage < this.pagination.totalPages) {
          this.pagination.currentPage = this.pagination.currentPage + 1;
        } else {
          this.pagination.currentPage = 1;
        }
      }, 3000);
    } else {
      clearInterval(this.intervalCurrent);
    }
  }
  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.getData();
  }
  search() {
    this.pagination.currentPage = 1;
    this.getData();
  }

  findMaterialName() {
    if (!this.functionUtility.checkEmpty(this.material_ID)) {
      this.inputService.findMaterialName(this.material_ID).subscribe((res) => {
        this.material_Name = res.materialName;
      });
    } else {
      this.material_Name = "";
    }
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
  changedSupplier(e: any): void {
    this.supplier_ID = e;
  }
  exportExcel() {
    if (this.status === "") {
      this.alertify.error(
        "Please select the value for the status are In or Out!"
      );
    } else if (this.status === "M") {
      this.alertify.error(
        "Export transfer file functionality is not available!"
      );
    } else {
      let isReport = true;
      let param;
      if (this.functionUtility.checkEmpty(this.fromDate)) {
        if (this.functionUtility.checkEmpty(this.toDate)) {
          isReport = true;
          param = {
            transac_Type: this.status,
            dateStart: "",
            dateEnd: "",
            po: this.mO_No,
            t2_Supplier_ID: this.supplier_ID
          };
        } else {
          isReport = false;
          this.alertify.error("Please enter start time!");
        }
      } else {
        if (this.functionUtility.checkEmpty(this.toDate)) {
          isReport = false;
          this.alertify.error("Please enter end time!");
        } else {
          isReport = true;
          let dateStart = this.functionUtility.getDateFormat(
            new Date(this.fromDate)
          );
          let dateEnd = this.functionUtility.getDateFormat(
            new Date(this.toDate)
          );
          param = {
            transac_Type: this.status,
            dateStart: dateStart,
            dateEnd: dateEnd,
            po: this.mO_No,
            t2_Supplier_ID: this.supplier_ID
          };
        }
      }
      if (isReport) {
        this.transferService.exportExcel(param);
      }
    }
  }
}
