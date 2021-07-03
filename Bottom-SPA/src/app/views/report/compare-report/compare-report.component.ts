import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { StockCompare } from '../../../_core/_models/stock-compare';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { CompareReportService } from '../../../_core/_services/compare-report.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';

@Component({
  selector: "app-compare-report",
  templateUrl: "./compare-report.component.html",
  styleUrls: ["./compare-report.component.scss"],
})
export class CompareReportComponent implements OnInit {
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
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
  receive_Date: string;
  bsConfig: Partial<BsDatepickerConfig>;
  stockCompares: StockCompare[] = [];
  constructor(private compareReportService: CompareReportService,
              private functionUtility: FunctionUtility,
              private spinnerService: NgxSpinnerService,
              private alertify: AlertifyService) {}

  ngOnInit(): void {
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: 'theme-blue',
        isAnimated: true,
        dateInputFormat: 'YYYY/MM/DD',
      }
    );
  }
  getData() {
    if(this.functionUtility.checkEmpty(this.receive_Date)) {
      this.alertify.error('Please option date!')
    } else {
      this.spinnerService.show();
      let receiveDate = this.functionUtility.getDateFormat(new Date(this.receive_Date));
      this.compareReportService.search(receiveDate, this.pagination.currentPage, this.pagination.itemsPerPage).subscribe((res) => {
        this.stockCompares = res.result;
        this.pagination = res.pagination;
        this.spinnerService.hide();
      }, error => {
        this.alertify.error(error);
        this.spinnerService.hide();
      });
    }
  }
  search() {
    this.pagination.currentPage = 1;
    this.getData();
  }
  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }
  exportExcel() {
    if(this.functionUtility.checkEmpty(this.receive_Date)) {
      this.alertify.error('Please option Date!');
    } else {
      if(this.stockCompares.length === 0) {
        this.alertify.error('No Data!');
      } else {
        let receiveDate = this.functionUtility.getDateFormat(new Date(this.receive_Date));
        this.compareReportService.exportExcel(receiveDate);
      }
    }
  }
}
