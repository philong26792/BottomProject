import { Component, OnInit } from "@angular/core";
import { AlertifyService } from "../../../_core/_services/alertify.service";
import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { Select2OptionData } from "ng-select2";
import { PackingListService } from "../../../_core/_services/packing-list.service";
import { InputService } from "../../../_core/_services/input.service";
import { NgxSpinnerService } from "ngx-spinner";
import { FunctionUtility } from "../../../_core/_utility/function-utility";
import { IntegrationInputModel } from "../../../_core/_models/intergration-input";
import { TransferService } from "../../../_core/_services/transfer.service";
import { OutputService } from "../../../_core/_services/output.service";
import { Router } from "@angular/router";
import { Pagination, PaginatedResult } from "../../../_core/_models/pagination";

@Component({
  selector: "app-integration-input",
  templateUrl: "./integration-input.component.html",
  styleUrls: ["./integration-input.component.css"],
})
export class IntegrationInputComponent implements OnInit {
  checkPrintLocation: string = "true";
  countParam = 0;
  isSubmit = false;
  disable = false;
  bsConfig: Partial<BsDatepickerConfig>;
  time_start: string;
  time_end: string;
  mO_No: string;
  supplier_ID: string;
  location: string;
  // Mảng để show ngoài màn hình
  integrationInputList: IntegrationInputModel[] = [];
  // All data khi search
  integrationAll: IntegrationInputModel[] = [];
  public supplierList: Array<Select2OptionData>;
  filterInput: any;

  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%",
  };
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
    private alertify: AlertifyService,
    private router: Router,
    private packingListService: PackingListService,
    private inputService: InputService,
    private spinnerService: NgxSpinnerService,
    private alertifyService: AlertifyService,
    private functionUtility: FunctionUtility,
    private transferService: TransferService,
    private outputService: OutputService,
  ) {}

  ngOnInit(): void {
    this.inputService.integrationsSource.subscribe(
      (res) => (this.integrationInputList = res)
    );
    this.inputService.isSubmitIntegrationSource.subscribe(
      (res) => (this.isSubmit = res)
    );
    this.getSupplier();
    if (this.isSubmit) {
      this.inputService.currentIntegrationParam.subscribe(
        (res) => (this.filterInput = res)
      );
      this.mO_No = this.filterInput.mO_No;
      this.supplier_ID = this.filterInput.supplier_ID;
      this.time_start = this.filterInput.from_Date;
      this.time_end = this.filterInput.to_Date;
    } else {
      this.getTimeNow();
      this.getData();
    }
    this.checkPrintLocationInLocalStorage();
  }
  getTimeNow() {
    const timeNow =
      new Date().getFullYear().toString() +
      "/" +
      (new Date().getMonth() + 1).toString() +
      "/" +
      new Date().getDate().toString();
    this.time_start = timeNow;
    this.time_end = timeNow;
    let form_date = this.functionUtility.getDateFormat(
      new Date(this.time_start)
    );
    let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
    this.filterInput = {
      supplier_ID: "",
      mO_No: "",
      from_Date: form_date,
      to_Date: to_date,
    };
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
  getData() {
    this.spinnerService.show();
    this.inputService
      .searchIntegration(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.filterInput
      )
      .subscribe((res: PaginatedResult<IntegrationInputModel[]>) => {
        this.spinnerService.hide();
        this.integrationAll = res.result;
        this.integrationInputList = this.integrationAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
        this.pagination = res.pagination;
        if (this.integrationInputList.length === 0) {
          this.alertifyService.error("No Data!");
        }
      });
  }
  search() {
    this.isSubmit = false;
    this.integrationInputList.length = 0;
    this.inputService.changeIntegrations([]);
    this.inputService.changeIntegrationParam([]);
    this.countParam = 0;
    if (this.time_start === null && this.time_end === null) {
      this.filterInput = {
        supplier_ID: this.supplier_ID,
        mO_No: this.mO_No,
        from_Date: this.time_start,
        to_Date: this.time_end,
      };
    } else {
      let form_date = this.functionUtility.getDateFormat(
        new Date(this.time_start)
      );
      let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
      this.filterInput = {
        supplier_ID: this.supplier_ID,
        mO_No: this.mO_No,
        from_Date: form_date,
        to_Date: to_date,
      };
    }
    this.inputService.changeIntegrationParam(this.filterInput);
    this.pagination.currentPage = 1;
    this.getData();
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.integrationInputList = this.integrationAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }
  submit() {
    this.isSubmit = true;
    let integrationParamSubmit = this.integrationAll.filter(
      (o) => o.rack_Location !== null && o.rack_Location !== "");
    this.inputService.changeIsSubmitIntegration(true);
    this.spinnerService.show();
    this.inputService
      .submitIntegration(integrationParamSubmit)
      .subscribe((res) => {
        if (res.result) {
          this.integrationInputList = integrationParamSubmit;
          this.inputService.changeIntegrations(
            this.integrationInputList
          );
          this.spinnerService.hide();
          this.alertifyService.success("Submit successed!");
        } else {
          this.alertifyService.error("Submit unsuccessed");
        }
      });
  }
  print(model: any) {
    this.outputService.changePrintQrCode("3");
    const paramPrintQrCodeAgain = [
      {
        qrCode_ID: model.qrCode_ID,
        qrCode_Version: model.qrCode_Version,
        mO_Seq: model.mO_Seq,
      },
    ];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(["/output/print-qrcode-again"]);
  }
  printAll() {
    let printQrCodeAgains = [];
    this.integrationInputList.forEach((item) => {
      if (item.rack_Location !== null && item.rack_Location != "") {
        const paramPrintQrCodeAgain = {
          qrCode_ID: item.qrCode_ID,
          qrCode_Version: item.qrCode_Version,
          mO_Seq: item.mO_Seq,
        };
        printQrCodeAgains.push(paramPrintQrCodeAgain);
      }
    });
    this.outputService.changeParamPrintQrCodeAgain(printQrCodeAgains);
    this.outputService.changePrintQrCode("3");
    this.router.navigate(["/output/print-qrcode-again"]);
  }
  checkRackExist(model: IntegrationInputModel, i: number) {
    if (!this.functionUtility.checkEmpty(model.rack_Location)) {
      let inputEvent = document.getElementById(model.qrCode_ID + i);
      model.rack_Location = model.rack_Location.toUpperCase();
      this.transferService
        .checkExistLocation(model.rack_Location)
        .subscribe((res) => {
          if (res === false) {
            this.integrationInputList.forEach((item) => {
              if (item.rack_Location === model.rack_Location) {
                item.rack_Location = null;
              }
            });
            this.alertify.error(
              "Rack Location does not exist please scan again!"
            );
          } else {
            this.integrationInputList.forEach((item) => {
              if (
                item.rack_Location === model.rack_Location &&
                model.rack_Location !== "" &&
                model.rack_Location !== null
              ) {
                item.rack_Location = item.rack_Location.toUpperCase();
              }
            });

            let checkRackOfV696;
            this.inputService
              .checkEnterRackInputIntergration(
                model.rack_Location,
                model.receive_No
              )
              .subscribe((res) => {
                if (res.result === "ok") {
                  checkRackOfV696 = true;
                } else if (res.result === "input-rack-A012") {
                  checkRackOfV696 = false;
                  this.alertify.error("Please enter rackLocation in RB Area");
                } else if (res.result === "input-rack-not-A012") {
                  checkRackOfV696 = false;
                  this.alertify.error(
                    "Please enter Rack Location of Bottom factory"
                  );
                }
                if (!checkRackOfV696) {
                  this.integrationAll.forEach((item) => {
                    if (
                      item.rack_Location === model.rack_Location &&
                      item.mO_No.trim() === model.mO_No.trim() &&
                      item.qrCode_ID.trim() === model.qrCode_ID.trim()
                    ) {
                      item.rack_Location = null;
                    }
                  });
                }
              });
          }
          inputEvent.blur();
        });
    }
  }
  checkPrintLocationInLocalStorage() {
    if (localStorage.getItem("checkPrintLocationIntergation") == null) {
      this.checkPrintLocation = "true";
      localStorage.setItem("checkPrintLocationIntergation", "true");
    } else {
      this.checkPrintLocation = localStorage.getItem(
        "checkPrintLocationIntergation"
      );
    }
  }

  changeCheckPrintLocation(e) {
    if (e.target.checked) {
      localStorage.setItem("checkPrintLocationIntergation", "true");
    } else {
      localStorage.setItem("checkPrintLocationIntergation", "false");
    }
  }
  ngOnDestroy(): void {
    this.inputService.changeIsSubmitIntegration(false);
  }
  cancle() {
    if (!this.isSubmit) {
      this.integrationInputList.map((obj) => {
        obj.rack_Location = "";
        return obj;
      });
    } else {
      this.isSubmit = false;
      this.countParam = 0;
      this.integrationInputList.length = 0;
      this.inputService.changeIsSubmitIntegration(false);
      this.inputService.changeIntegrations([]);
      this.inputService.changeIntegrationParam([]);
    }
  }

  ngAfterContentChecked() {
    this.countParam = 0;
    this.integrationAll.forEach((item) => {
      if (
        item.rack_Location != "" &&
        item.rack_Location != null &&
        !item.rack_Location.trim().length === false
      ) {
        this.countParam = this.countParam + 1;
      }
    });
  }
}
