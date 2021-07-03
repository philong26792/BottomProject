import { Component, OnInit } from '@angular/core';
import { KanbanByPoDetail } from '../../../../_core/_models/kanban-by-po-detail';
import { KanbanByPoService } from '../../../../_core/_services/kanban-by-po.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { FunctionUtility } from '../../../../_core/_utility/function-utility';
import { Router } from '@angular/router';
@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})
export class DetailComponent implements OnInit {
  listKanbanByPoDetail: KanbanByPoDetail[] = [];
  kanbanByPo: any = [];
  time: Date = new Date();
  nodata: boolean = false;
  listColorChart: any = [];
  paramSearch: any = [];

  constructor(private kanbanByPoService: KanbanByPoService,
    private spinner: NgxSpinnerService,
    private functionUtility: FunctionUtility,
    private router: Router) { }

  ngOnInit() {
    setInterval(() => {
      this.time = new Date();
    }, 1000);

    this.kanbanByPoService.currentKanbanByPo.subscribe(res => {
      this.kanbanByPo = res;
    }).unsubscribe();

    this.kanbanByPoService.currentParamSearch.subscribe(res => {
      this.paramSearch = res;
    }).unsubscribe();

    this.getData();
  }

  getData() {
    this.spinner.show();
    this.kanbanByPoService.getDetailKanbanByPo(this.kanbanByPo.mO_No, this.kanbanByPo.mO_Seq).subscribe(res => {
      this.listKanbanByPoDetail = res;
      if (this.listKanbanByPoDetail.length === 0) {
        this.nodata = true;
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
    });
  }

  exportExcel() {
    this.spinner.show();
    this.kanbanByPoService.exportExcelDetail(this.kanbanByPo.mO_No, this.kanbanByPo.mO_Seq);
    this.spinner.hide();
  }

  exportExcelDetailWithSize() {
    const d1 = this.paramSearch.dateStart == null ? '' : this.functionUtility.getDateFormat(this.paramSearch.dateStart);
    const d2 = this.paramSearch.dateEnd == null ? '' : this.functionUtility.getDateFormat(this.paramSearch.dateEnd);
    this.kanbanByPoService.exportExcelDetailWithSize(this.paramSearch.dateType, d1, d2, this.kanbanByPo.cell, this.kanbanByPo.mO_No,
      this.kanbanByPo.article, this.kanbanByPo.model_Name, this.kanbanByPo.mO_Seq);
  }

  gotoDetailByReceivingType(kanbanByPoDetail: KanbanByPoDetail) {
    this.kanbanByPoService.changeKanbanByPoDetail(kanbanByPoDetail);
    this.router.navigateByUrl('/kanban/by-po/detail-by-receiving-type');
  }
}
