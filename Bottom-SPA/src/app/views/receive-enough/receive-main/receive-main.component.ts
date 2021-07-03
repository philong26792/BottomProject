import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { NgxSpinnerService } from "ngx-spinner";
import { PaginatedResult, Pagination } from "../../../_core/_models/pagination";
import { AlertifyService } from "../../../_core/_services/alertify.service";
import { InputService } from "../../../_core/_services/input.service";
import { ReceivingService } from "../../../_core/_services/receiving.service";
import { FunctionUtility } from "../../../_core/_utility/function-utility";
import { MaterialParam } from "../../../_core/_viewmodels/material-param";
import { Material2Model } from "../../../_core/_viewmodels/material2-model";

@Component({
  selector: "app-receive-main",
  templateUrl: "./receive-main.component.html",
  styleUrls: ["./receive-main.component.css"],
})
export class ReceiveMainComponent implements OnInit {
  isShowSubmit = false;
  materialLists: Material2Model[] = [];
  materialListAll: Material2Model[] = [];
  materialParam: MaterialParam;
  pagination: Pagination;
  disable = false;
  bsConfig: Partial<BsDatepickerConfig>;
  purchase_No: string;
  status: string = "all";
  mO_No: string = "";
  time_start: string;
  time_end: string;
  checkboxAll: boolean = false;
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
    private receivingService: ReceivingService,
    private inputService: InputService,
    private router: Router,
    private spinnerService: NgxSpinnerService,
    private alertifyService: AlertifyService,
    private functionUtility: FunctionUtility
  ) {}

  ngOnInit(): void {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 0,
      totalPages: 0,
    };
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: "theme-blue",
        isAnimated: true,
        dateInputFormat: "YYYY/MM/DD",
      }
    );

    this.receivingService.currentParam.subscribe(
      (res) => (this.materialParam = res)
    );
    if (this.materialParam !== undefined && this.materialParam !== null) {
      this.purchase_No = this.materialParam.purchase_No.toString().trim();
      if (this.materialParam.from_Date === null) {
        this.time_start = "";
      } else {
        this.time_start = this.materialParam.from_Date;
      }

      if (this.materialParam.to_Date === null) {
        this.time_end = "";
      } else {
        this.time_end = this.materialParam.to_Date;
      }

      if (
        this.materialParam.from_Date === undefined &&
        this.materialParam.mO_No === undefined
      ) {
      } else {
        this.search();
      }
    }
    this.inputService.clearDataChangeMenu();
  }
  getData() {
    this.spinnerService.show();
    this.receivingService.search(this.pagination.currentPage,this.pagination.itemsPerPage,this.materialParam)
      .subscribe(
        (res: PaginatedResult<Material2Model[]>) => {
          this.materialListAll = res.result;
          this.materialLists = this.materialListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
          this.materialLists.map((item) => {
            item.checkInput = false;
            return item;
          });
          this.pagination = res.pagination;
          this.spinnerService.hide();
          if (this.materialLists.length === 0) {
            this.alertifyService.error("No Data!");
          }
        },
        (error) => {
          this.alertifyService.error(error);
        }
      );
  }
  search() {
    let checkSearch = true;
    if (this.functionUtility.checkEmpty(this.time_start)) {
      if (this.functionUtility.checkEmpty(this.purchase_No)) {
        checkSearch = false;
        this.alertifyService.error(
          "Please option Expected Delivery Date Or Transfer No!"
        );
      } else {
        this.materialParam = {
          mO_No: this.mO_No,
          purchase_No: this.purchase_No,
          from_Date: null,
          to_Date: null,
          status: this.status,
        };
      }
    } else{
      if (this.functionUtility.checkEmpty(this.time_end)) {
        checkSearch = false;
        this.alertifyService.error("Please option time end!");
      } else {
          checkSearch = true;
          let form_date = this.functionUtility.getDateFormat(new Date(this.time_start));
          let to_date = this.functionUtility.getDateFormat(new Date(this.time_end));
          this.materialParam = {
            mO_No: this.mO_No,
            purchase_No: this.purchase_No,
            from_Date: form_date,
            to_Date: to_date,
            status: this.status,
          };
      }
    }
    if (checkSearch) {
      this.pagination.currentPage = 1;
      this.receivingService.changeParam(this.materialParam);
      this.getData();
    }
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.materialLists = this.materialListAll.slice((this.pagination.currentPage - 1) * this.pagination.itemsPerPage, this.pagination.itemsPerPage * this.pagination.currentPage);
  }
  cancle() {
    this.mO_No = "";
    this.purchase_No = "";
    this.status = "all";
    this.isShowSubmit = false;
    this.materialListAll.length = 0;
    this.materialLists.length = 0;
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 0,
      totalPages: 0,
    };
  }
  submit() {
    let param;
    param = this.materialListAll.filter(
      (x) => x.checkInput === true && x.status.trim() === "N"
    );
    this.spinnerService.show();
    this.receivingService.receivingMaterial(param).subscribe((res) => {
      this.receivingService.changeReceive(res);
      this.alertifyService.success("Submit successed!");
      this.spinnerService.hide();
      this.router.navigate(["/receive/record"]);
    });
  }
  checkAll(e) {
    if(e.target.checked) {
      this.materialListAll.map(obj => {
        obj.checkInput = true;
      });
    } else {
      this.materialListAll.map(obj => {
        obj.checkInput = false;
      });
    }
  }

  ngAfterContentChecked() {
    if(this.materialListAll.length > 0) {
      let countMaterialStatusN = this.materialListAll.filter(x => x.status.trim() === 'N').length;
      let countCheckInput = this.materialListAll.filter(x => x.status.trim() === 'N' && x.checkInput).length;
      if(countCheckInput === countMaterialStatusN &&  countCheckInput > 0) {
        this.checkboxAll = true;
      } else {
        this.checkboxAll = false;
      }
  
      if(countCheckInput >0 ) {
        this.isShowSubmit = true;
      } else {
        this.isShowSubmit = false;
      }
    }
  }
}
