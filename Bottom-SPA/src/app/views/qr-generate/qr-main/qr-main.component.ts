import { Component, OnInit } from "@angular/core";
import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { Router } from "@angular/router";
import { AlertifyService } from "../../../_core/_services/alertify.service";
import { PackingListService } from "../../../_core/_services/packing-list.service";
import { PackingList } from "../../../_core/_models/packingList";
import { Pagination, PaginatedResult } from "../../../_core/_models/pagination";
import { QrcodeMainService } from "../../../_core/_services/qrcode-main.service";
import { PackingSearch } from "../../../_core/_viewmodels/packing-search";
import { InputService } from "../../../_core/_services/input.service";
import { FunctionUtility } from "../../../_core/_utility/function-utility";
import { NgxSpinnerService } from "ngx-spinner";
import { Select2OptionData } from "ng-select2";
import { QrCodeGenerateModel } from '../../../_core/_viewmodels/qr-generate-model';
@Component({
  selector: "app-qr-main",
  templateUrl: "./qr-main.component.html",
  styleUrls: ["./qr-main.component.scss"],
})
export class QrMainComponent implements OnInit {
  pagination: Pagination;
  bsConfig: Partial<BsDatepickerConfig>;
  disable = false;
  time_start: string;
  time_end: string;
  fromDate = new Date();
  toDate = new Date();
  checkboxAll: boolean = false;
  packingSearchParam: PackingSearch;
  qrCodeGenerateList: QrCodeGenerateModel[];
  qrCodeGenerateAll: QrCodeGenerateModel[];
  supplier_ID: string;
  public supplierList: Array<Select2OptionData>;
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%",
  };
  mO_No: string;
  alerts: any = [
    {
      type: "success",
      msg: `You successfully read this important alert message.`,
    },
    {
      type: "info",
      msg: `This alert needs your attention, but it's not super important.`,
    },
    {
      type: "danger",
      msg: `Better check yourself, you're not looking too good.`,
    },
  ];
  constructor(
    private packingListService: PackingListService,
    private qrcodeService: QrcodeMainService,
    private router: Router,
    private inputService: InputService,
    private alertifyService: AlertifyService,
    private functionUtility: FunctionUtility,
    private spinnerService: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.getListSupplier();
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 0,
      totalPages: 0,
    };
    this.bsConfig = Object.assign({}, { containerClass: "theme-blue" });
    this.getDataLoadPage();
    this.inputService.clearDataChangeMenu();
  }
  getData() {
    this.spinnerService.show();
    this.packingListService.search(this.pagination.currentPage,this.pagination.itemsPerPage,this.packingSearchParam)
      .subscribe(
        (res: PaginatedResult<PackingList[]>) => {
          this.qrCodeGenerateAll = res.result.map(obj => ({
            ...obj, checkInput: false
          }));
          this.qrCodeGenerateList = this.qrCodeGenerateAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
          this.pagination = res.pagination;
          if (this.qrCodeGenerateAll.length === 0) {
            this.alertifyService.error("No Data!");
          }
          this.spinnerService.hide();
        },
        (error) => {
          this.alertifyService.error(error);
          this.spinnerService.hide();
        }
      );
  }
  getDataLoadPage() {
    const timeNow = this.functionUtility.getToDay();
    this.time_start = timeNow;
    this.time_end = timeNow;
    let timeConvert = this.functionUtility.getDateFormat(new Date(timeNow));
    this.packingSearchParam = {
      supplier_ID: this.supplier_ID,
      mO_No: "",
      from_Date: timeConvert,
      to_Date: timeConvert,
    };
    this.getData();
  }
  search() {
    this.checkboxAll = false;
    this.pagination.currentPage = 1;
    let checkSearch = true;
    if (this.time_start !== null) {
      if (this.time_end === null) {
        checkSearch = false;
        this.alertifyService.error("Please option time end!");
      }
    } else {
      if (this.time_end !== null) {
        checkSearch = false;
        this.alertifyService.error("Please option time start!");
      }
    }
    if (this.time_start === null) {
      this.packingSearchParam = {
        supplier_ID: this.supplier_ID,
        mO_No: this.mO_No,
        from_Date: null,
        to_Date: null,
      };
    } else {
      let form_date = this.functionUtility.getDateFormat(new Date(this.time_start));
      let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
      this.packingSearchParam = {
        supplier_ID: this.supplier_ID,
        mO_No: this.mO_No,
        from_Date: form_date,
        to_Date: to_date,
      };
    }
    if(checkSearch) {
      this.getData();
    }
  }
  onCheckboxChange(e, item) {
    const countGenQrCodeNotChecked = this.qrCodeGenerateAll.filter(x => x.checkInput !== true).length;
    if(countGenQrCodeNotChecked === 0) {
      this.checkboxAll = true;
    } else {
      this.checkboxAll = false;
    }
  }
  // Khi stick chọn all checkbox
  checkAll(e) {
    if(e.target.checked) {
      this.qrCodeGenerateAll.map(obj => {
        obj.checkInput = true;
        return obj;
      });
    } else {
      this.qrCodeGenerateAll.map(obj => {
        obj.checkInput = false;
        return obj;
      })
    }
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.qrCodeGenerateList = this.qrCodeGenerateAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }
  // genare QrCode
  pageQrCode() {
    let genQrCodeInCheck = this.qrCodeGenerateAll.filter(x => x.checkInput === true);
    if(genQrCodeInCheck.length > 0) {
      this.spinnerService.show();
      let paramGenerateQrCode = genQrCodeInCheck.map(x => x.receive_No);
      this.qrcodeService.generateQrCode(paramGenerateQrCode).subscribe(
            (res) => {
              this.spinnerService.hide();
              this.alertifyService.success("Generate QRCode successed!");
              this.router.navigate(["/qr/body"]);
              this.spinnerService.hide();
            },
            (error) => {
              this.alertifyService.error(error);
              this.spinnerService.hide();
            }
          );
    } else {
      this.alertifyService.error("Please check checkbox!");
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
}
