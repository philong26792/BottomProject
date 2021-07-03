import { Component, OnInit } from "@angular/core";
import { KanbanByrackService } from "../../../../_core/_services/kanban-byrack.service";
import { PoListT3 } from "../../../../_core/_models/po-list-t3";
import { Location } from "@angular/common";
import { NgxSpinnerService } from "ngx-spinner";
import { Pagination } from '../../../../_core/_models/pagination';

@Component({
  selector: "app-po-list-t3",
  templateUrl: "./po-list-t3.component.html",
  styleUrls: ["./po-list-t3.component.scss"],
})
export class PoListT3Component implements OnInit {
  time: Date = new Date();
  listPo: PoListT3[] = [];
  rackLocation: string = "B-01-04";
  poQty: number = 0;
  ttl: number = 0;
  nodata: boolean = false;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  };

  constructor(
    private kanbanByRackService: KanbanByrackService,
    private location: Location,
    private spinner: NgxSpinnerService
  ) { }

  ngOnInit() {
    setInterval(() => {
      this.time = new Date();
    }, 1000);
    this.kanbanByRackService.currentRack.subscribe((res) => {
      this.rackLocation = res;
    });
    this.getData();
  }

  back() {
    this.location.back();
  }

  getData() {
    this.spinner.show();
    this.kanbanByRackService.getPoListT3(this.rackLocation, this.pagination.currentPage, this.pagination.itemsPerPage).subscribe(
      (res) => {
        this.listPo = res.result.data;
        this.pagination = res.pagination;
        if (this.listPo.length === 0) {
          this.nodata = true;
        } else {
          this.ttl = this.listPo[0].ttL_PRS;
          this.poQty =  res.result.poQty;
        }
        this.spinner.hide();
      },
      (error) => {
        this.spinner.hide();
      }
    );
  }

  exportExcel() {
    this.kanbanByRackService.exportExcelPoListT3(this.rackLocation);
  }

  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.getData();
  }
}
