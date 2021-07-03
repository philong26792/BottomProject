import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Select2OptionData } from "ng-select2";
import { NgxSpinnerService } from "ngx-spinner";
import { ModifyQrCodeMain } from "../../../_core/_models/modify-qrcode-main";
import { AlertifyService } from "../../../_core/_services/alertify.service";
import { ModifyQrcodeService } from "../../../_core/_services/modify-qrcode.service";
import { PackingListService } from "../../../_core/_services/packing-list.service";
import { FunctionUtility } from "../../../_core/_utility/function-utility";
import {
  BatchDetail,
  ReasonDetail,
} from "../../../_core/_viewmodels/modify-store/reason-detail";

@Component({
  selector: "app-modify-qrcode-main",
  templateUrl: "./modify-qrcode-main.component.html",
  styleUrls: ["./modify-qrcode-main.component.scss"],
})
export class ModifyQrcodeMainComponent implements OnInit {
  modifyQrCodeList: ModifyQrCodeMain[] = [];
  supplierList: Array<Select2OptionData>;
  disable = false;
  supplier_ID: string = "All";
  mO_No: string;
  qrCodeID: string = "";
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
    private packingListService: PackingListService,
    private modifyQrCodeService: ModifyQrcodeService,
    private alertify: AlertifyService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private functionUtility: FunctionUtility
  ) {}

  ngOnInit() {
    this.getListSupplier();
  }
  search() {
    if (this.functionUtility.checkEmpty(this.mO_No)) {
      this.alertify.error("Please enter Plan No!");
    } else {
      this.spinner.show();
      this.modifyQrCodeService
        .search(this.mO_No, this.supplier_ID, this.qrCodeID)
        .subscribe((res) => {
          if (res.length === 0) {
            this.alertify.error("No Data!");
          }
          this.modifyQrCodeList = res;
          this.spinner.hide();
        });
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
  changeQrcodeDetail(item: ModifyQrCodeMain) {
    this.modifyQrCodeService.changeQrCodeModifyMain(item);
    this.router.navigate(["/modify-store/no-by-batch-out"]);
  }

  getPlanNoByQrCodeId() {
    if (this.qrCodeID.length == 15) {
      this.modifyQrCodeService
        .getPlanNoByQrCodeId(this.qrCodeID)
        .subscribe((res) => {
          this.mO_No = res.planNo;
        });
    }
  }
}
