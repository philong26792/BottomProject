import { KanbanService } from './../../../../_core/_services/kanban.service';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { KanbanByCategoryDetailByPo } from '../../../../_core/_models/kanban-by-category-detail-by-po';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-detail-po-no',
  templateUrl: './detail-po-no.component.html',
  styleUrls: ['./detail-po-no.component.scss']
})
export class DetailPoNoComponent implements OnInit {
  codeId: string = '';
  codeName: string = '';
  toolCode: string = '';
  po: string = '';
  TTLPRS: number = 0;
  time: Date = new Date();
  nodata: boolean = false;
  listKanbanByCategoryDetailByPo: KanbanByCategoryDetailByPo[] =  [];
  constructor(private router: Router, private route: ActivatedRoute, private kanbanService: KanbanService,
    private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.codeId = this.route.snapshot.params['codeId'];
    this.toolCode = this.route.snapshot.params['toolCode'];
    this.po = this.route.snapshot.params['po'];
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
    this.kanbanService.getKanbanByCategoryDetailByPo(this.codeId, this.toolCode, this.po).subscribe(res => {
      this.listKanbanByCategoryDetailByPo = res;
      if (this.listKanbanByCategoryDetailByPo.length == 0) {
        this.nodata = true;
      }
      this.TTLPRS = res.reduce((qty, i) => {
        return (qty += i.qty);
      }, 0);
      this.spinner.hide();
    });
  }
  back() {
    this.router.navigate(['/kanban/by-category/detail/tool-code/' + this.codeId  + '/' + this.toolCode ]);
  }

  exportExcel() {
    this.kanbanService.exportExcelKanBanByCategoryDetailByPo(this.codeId, this.toolCode, this.po);
  }
}
