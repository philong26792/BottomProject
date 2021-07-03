import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { KanbanByPo } from '../../../_core/_models/kanban-by-po';
import { Pagination } from '../../../_core/_models/pagination';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { ReportService } from '../../../_core/_services/report.service';
import { PackingListService } from '../../../_core/_services/packing-list.service';
import { Select2OptionData } from 'ng-select2';
import { InputService } from '../../../_core/_services/input.service';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-receive-material',
  templateUrl: './receive-material.component.html',
  styleUrls: ['./receive-material.component.scss']
})
export class ReceiveMaterialComponent implements OnInit {
  bsConfig: Partial<BsDatepickerConfig>;
  listKanbanByPo: KanbanByPo[] = [];
  nodata: boolean = false;
  listLine: any[] = [];
  public supplierList: Array<Select2OptionData>;
  optionsSelectSupplier = {
    placeholder: "Select supplier...",
    allowClear: true,
    width: "100%"
  };
  disable = false;
  supplier: string = '';
  status: string = 'All';
  dateType: number = 1;
  dateStart: Date = new Date();
  dateEnd: Date = new Date();
  moNo: string = '';
  planNo: string = '';
  batch: string = '';
  article: string = '';
  tooling: string = '';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(private reportService: ReportService,
    private packingListService: PackingListService,
    private inputService: InputService,
    private alertifyService: AlertifyService,
    private spiner: NgxSpinnerService,
    private functionUtility: FunctionUtility) { }

  ngOnInit() {
    this.bsConfig = Object.assign({}, {
      containerClass: 'theme-blue',
      isAnimated: true,
      dateInputFormat: 'YYYY/MM/DD',
    }
    );

    this.reportService.currentParamSearch.subscribe(res => {
      this.dateType = res.dateType;
      this.dateStart = res.dateStart;
      this.dateEnd = res.dateEnd;
      this.moNo = res.moNo;
      this.batch = res.batch;
      this.supplier = res.supplier;
      this.pagination.currentPage = res.currentPage;
    });

    this.getSupplier();
    this.inputService.clearDataChangeMenu();
  }


  getSupplier() {
    this.packingListService.supplierList().subscribe(res => {
      this.supplierList = res.map(obj => {
        let supplier = {
          id: obj.supplier_No,
          text: obj.supplier_No + '-' + obj.supplier_Name
        }
        return supplier;
      });
      this.supplierList.unshift({
        id: "All",
        text: "All Supplier"
      });
    });
  }
  changedSupplier(e: any): void {
    this.supplier = e;
  }
  exportExcel() {
    debugger
    if(this.dateStart > this.dateEnd) {
      this.alertifyService.error('Please option Start date less than end date');
    } else {
      this.spiner.show();
      const d1 = this.functionUtility.getDateFormat(this.dateStart);
      const d2 = this.functionUtility.getDateFormat(this.dateEnd);
      this.reportService.exportExcelMaterialReceive(this.dateType, d1, d2, this.moNo, this.batch, this.supplier, this.status, this.article, this.tooling);
    }
  }

  clear() {
    this.reportService.currentParamSearch.subscribe(res => {
      this.dateType = res.dateType;
      this.dateStart = res.dateStart;
      this.dateEnd = res.dateEnd;
      this.moNo = res.moNo;
      this.batch = res.batch;
      this.supplier = res.supplier;
      this.status = 'All';
      this.article = '';
      this.tooling = '';
      this.pagination.currentPage = res.currentPage;
    });
  }
}
