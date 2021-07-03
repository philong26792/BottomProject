import { Component, OnInit } from "@angular/core";
import { Pagination } from "../../../../_core/_models/pagination";
import { Router, ActivatedRoute } from "@angular/router";
import { KanbanService } from "../../../../_core/_services/kanban.service";
import { KanbanByCategoryDetail } from "../../../../_core/_models/kanban-by-category-detail";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-detail",
  templateUrl: "./detail.component.html",
  styleUrls: ["./detail.component.scss"],
})
export class DetailComponent implements OnInit {
  codeId: string = "";
  codeName: string = "";
  TTLPRS: number = 0;
  time: Date = new Date();
  nodata: boolean = false;
  listKanbanByCategoryDetail: KanbanByCategoryDetail[] = [];
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private kanbanService: KanbanService,
    private spinner: NgxSpinnerService
  ) { }

  ngOnInit() {
    this.codeId = this.route.snapshot.params["codeId"];
    this.kanbanService.currentCodeName.subscribe((res) => {
      this.codeName = res;
    });
    this.getData();
    setInterval(() => {
      this.time = new Date();
    }, 1000);
  }

  getData() {
    this.spinner.show();
    this.kanbanService
      .getKanbanByCategoryDetail(this.codeId, this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((res) => {
        this.listKanbanByCategoryDetail = res.result;
        this.pagination = res.pagination;
        if (this.listKanbanByCategoryDetail.length === 0) {
          this.nodata = true;
        }
        else {
          this.TTLPRS = this.listKanbanByCategoryDetail[0].ttL_PRS;
        }
        this.spinner.hide();
      });
  }

  detailByToolCode(toolCode: string) {
    this.router.navigate([
      "/kanban/by-category/detail/tool-code/" + this.codeId + "/" + toolCode,
    ]);
  }

  exportExcel() {
    this.kanbanService.exportExcelKanBanByCategoryDetail(this.codeId);
  }
  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }
}
