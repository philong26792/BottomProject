import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { KanbanService } from '../../../../_core/_services/kanban.service';
import { KanbanByCategoryDetailByToolCode } from '../../../../_core/_models/kanban-by-category-detai-by-tool-code';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../../_core/_models/pagination';

@Component({
  selector: 'app-detail-tool-code',
  templateUrl: './detail-tool-code.component.html',
  styleUrls: ['./detail-tool-code.component.scss']
})
export class DetailToolCodeComponent implements OnInit {
  codeId: string = '';
  codeName: string = '';
  toolCode: string = '';
  TTLPRS: number = 0;
  time: Date = new Date();
  nodata: boolean = false;
  listKanbanByCategoryDetailByToolCode: KanbanByCategoryDetailByToolCode[] = [];
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(private router: Router, private route: ActivatedRoute, private kanbanService: KanbanService,
    private spinner: NgxSpinnerService) { }


  ngOnInit() {
    this.codeId = this.route.snapshot.params['codeId'];
    this.toolCode = this.route.snapshot.params['toolCode'];
    this.kanbanService.currentCodeName.subscribe(res => {
      this.codeName = res;
    });
    this.getData();
    setInterval(() => {
      this.time = new Date();
    }, 1000);
  }

  getData() {
    this.spinner.show();
    this.kanbanService.getKanbanByCategoryDetailByToolCode(this.codeId, this.toolCode, this.pagination.currentPage,
      this.pagination.itemsPerPage)
      .subscribe(res => {
        this.listKanbanByCategoryDetailByToolCode = res.result;
        this.pagination = res.pagination;
        if (this.listKanbanByCategoryDetailByToolCode.length === 0) {
          this.nodata = true;
        }
        else {
          this.TTLPRS = this.listKanbanByCategoryDetailByToolCode[0].ttL_PRS;
        }
        this.spinner.hide();
      });
  }
  back() {
    this.router.navigate(['/kanban/by-category/detail/' + this.codeId]);
  }

  detailByPo(po: string) {
    this.router.navigate(['/kanban/by-category/detail/po-no/' + this.codeId + '/' + this.toolCode + '/' + po]);
  }

  exportExcel() {
    this.kanbanService.exportExcelKanBanByCategoryDetailByToolcode(this.codeId, this.toolCode);
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }

}
