import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { KanbanByPo } from '../../../../_core/_models/kanban-by-po';
import { KanbanByPoService } from '../../../../_core/_services/kanban-by-po.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../../_core/_models/pagination';
import { Router } from '@angular/router';
import { FunctionUtility } from '../../../../_core/_utility/function-utility';
import { AlertifyService } from '../../../../_core/_services/alertify.service';
import { Select2OptionData } from 'ng-select2';
@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {
  bsConfig: Partial<BsDatepickerConfig>;
  time: Date = new Date();
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
  line: string = '';
  supplier: string = '';
  dateType: number = 1;
  kind: number = 0;
  dateStart: Date = null;
  dateEnd: Date = null;
  moNo: string = '';
  article: string = '';
  modelName: string = '';
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(private kanbanByPoService: KanbanByPoService,
    private spinner: NgxSpinnerService,
    private router: Router,
    private functionUtility: FunctionUtility,
    private alertifyService: AlertifyService) { }
  ngOnInit() {
    setInterval(() => {
      this.time = new Date();
    }, 1000);
    this.bsConfig = Object.assign(
      {},
      {
        containerClass: 'theme-blue',
        isAnimated: true,
        dateInputFormat: 'YYYY/MM/DD',
      }
    );

    this.kanbanByPoService.currentParamSearch.subscribe(res => {
      this.dateType = res.dateType;
      this.dateStart = res.dateStart;
      this.dateEnd = res.dateEnd;
      this.line = res.line;
      this.moNo = res.moNo;
      this.supplier = res.supplier;
      this.article = res.article;
      this.modelName = res.modelName;
      this.kind = res.kind;
      this.pagination.currentPage = res.currentPage;
    });

    this.getLine();
    this.getSupplier();

    this.getData();
  }

  getData() {
    this.spinner.show();
    const d1 = this.dateStart == null ? '' : this.functionUtility.getDateFormat(this.dateStart);
    const d2 = this.dateEnd == null ? '' :  this.functionUtility.getDateFormat(this.dateEnd);
    this.kanbanByPoService.search(this.pagination.currentPage, this.pagination.itemsPerPage, this.dateType,
      d1, d2, this.line, this.moNo, this.supplier, this.article, this.kind, this.modelName)
      .subscribe(res => {
        this.listKanbanByPo = res.result;
        this.pagination = res.pagination;
        if (this.listKanbanByPo.length === 0) {
          this.nodata = true;
        }
        else {
          this.nodata = false;
        }

        this.spinner.hide();
      }, error => {
        this.alertifyService.error(error);
        this.spinner.hide();
      });
  }

  search() {
    if (this.moNo == '' && ((this.dateStart == null && this.dateEnd == null) || ((this.dateStart != null && this.dateEnd == null) || (this.dateStart == null && this.dateEnd != null)))) {
      this.alertifyService.error('Please choose Date or PO');
      return;
    }
    else if (this.moNo != '' && ((this.dateStart != null && this.dateEnd == null) || (this.dateStart == null && this.dateEnd != null))) {
      this.alertifyService.error('Please choose Date Start and Date End');
      return;
    }
    this.pagination.currentPage = 1;
    this.getData();
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }

  detail(kanbanByPo) {
    const paramSearch = {
      dateType: this.dateType,
      dateStart: this.dateStart,
      dateEnd: this.dateEnd,
      line: this.line,
      moNo: this.moNo,
      supplier: this.supplier,
      article: this.article,
      modelName: this.modelName,
      kind: this.kind,
      currentPage: this.pagination.currentPage
    };
    this.kanbanByPoService.changeParamSearch(paramSearch);
    this.kanbanByPoService.changeKanbanByPo(kanbanByPo);
    this.router.navigate(['/kanban/by-po/detail']);
  }

  getLine() {
    this.kanbanByPoService.getLine().subscribe(res => {
      this.listLine = res.map(item => {
        return { id: item, text: item };
      });

      this.listLine.unshift({ id: '', text: 'All Cell' });
    });
  }

  getSupplier() {
    this.kanbanByPoService.getSupplier().subscribe(res => {
      this.supplierList = res.map(item => {
        return { id: item.supplierNo, text: item.supplierNo + ' - ' + item.supplierName };
      });

      this.supplierList.unshift({ id: 'All', text: 'All Supplier' });
    });
  }
  changedSupplier(e: any): void {
    this.supplier = e;
  }
  exportExcel() {
    if (this.moNo == '' && (this.dateStart == null && this.dateEnd == null)) {
      this.alertifyService.error('Please choose Date or PO');
      return;
    }
    else if (this.moNo != '' && ((this.dateStart != null && this.dateEnd == null) || (this.dateStart == null && this.dateEnd != null))) {
      this.alertifyService.error('Please choose Date Start and Date End');
      return;
    }
    else if (this.moNo == '' && ((this.dateStart != null && this.dateEnd == null) || (this.dateStart == null && this.dateEnd != null))) {
      this.alertifyService.error('Please choose Date Start and Date End');
      return;
    }
    
    const d1 = this.dateStart == null ? '' : this.functionUtility.getDateFormat(this.dateStart);
    const d2 = this.dateEnd == null ? '' :  this.functionUtility.getDateFormat(this.dateEnd);
    if (this.kind == 0) {
      this.kanbanByPoService.exportExcelMain(this.dateType, d1, d2, this.line, this.moNo,
        this.supplier, this.article, this.modelName);
    }
    else {
      this.kanbanByPoService.exportExcelMainDetail(this.dateType, d1, d2, this.line, this.moNo,
        this.supplier, this.article, this.modelName);

    }
  }

  back() {
    const paramSearch = {
      dateType: 1,
      dateStart: new Date(),
      dateEnd: new Date(),
      line: '',
      moNo: '',
      supplier: '',
      article: '',
      modelName: '',
      kind: 0,
      currentPage: 1
    };
    this.kanbanByPoService.changeParamSearch(paramSearch);
    this.router.navigate(['/kanban/']);
  }
}
