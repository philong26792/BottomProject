import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { KanBanRackDetail } from '../../../../_core/_viewmodels/kanban-rack-detail';
import { KanbanByrackService } from '../../../../_core/_services/kanban-byrack.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { AlertifyService } from '../../../../_core/_services/alertify.service';
import { Pagination } from '../../../../_core/_models/pagination';

@Component({
  selector: 'app-po',
  templateUrl: './po.component.html',
  styleUrls: ['./po.component.scss']
})
export class PoComponent implements OnInit {
  time: Date = new Date();
  buildId: string = '';
  codeName: string = '';
  pO_Qty: number = 0;
  transacted_Qty_Sum: number = 0;
  rackDetails: KanBanRackDetail[] = [];
  rackLocation: string = '';
  noData = false;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(private router: Router, private route: ActivatedRoute,
    private alertifyService: AlertifyService,
    private rackService: KanbanByrackService,
    private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.spinner.show();
    setInterval(() => {
      this.time = new Date();
    }, 1000);
    this.buildId = this.route.snapshot.params['codeId'];
    this.codeName = this.route.snapshot.params['code_Ename'];
    this.rackLocation = this.route.snapshot.params['rack'];
    this.getData();
  }
  getData() {
    this.rackService.getKanbanRackDetail(this.rackLocation, this.pagination.currentPage, this.pagination.itemsPerPage).subscribe(res => {
      this.spinner.hide();
      this.rackDetails = res.result.data;
      this.pagination = res.pagination;
      if (this.rackDetails.length !== 0) {
        this.transacted_Qty_Sum = this.rackDetails[0].ttL_PRS;
        this.pO_Qty =  res.result.poQty;
      } else {
        this.noData = true;
      }
    }, error => {
      this.alertifyService.error(error);
    });
  }
  exportExcelRackDetail() {
    this.rackService.exportExcelRackDetailT2(this.rackLocation);
  }
  back() {
    this.router.navigate(['/kanban/by-rack/detail/' + this.buildId]);
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }
}
