import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Select2OptionData } from 'ng-select2';
import { NgxSpinnerService } from 'ngx-spinner';
import { Pagination } from '../../../_core/_models/pagination';
import { SettingReason } from '../../../_core/_models/setting-reason';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { SettingReasonService } from '../../../_core/_services/setting-reason.service';

@Component({
  selector: 'app-list-reason',
  templateUrl: './list-reason.component.html',
  styleUrls: ['./list-reason.component.scss']
})
export class ListReasonComponent implements OnInit {
  listReason: SettingReason[] = [];
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0
  }
  reasonName: string = "";
  trans_toHP: string = 'all';
  listReasonCode: Array<Select2OptionData>;
  reasonCode: string = 'all';
  constructor(
    private _service: SettingReasonService,
    private _router: Router,
    private _alert: AlertifyService,
    private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.getReasonCode();
    this.getAllReason();
  }
  getAllReason(pagNumber = 1) {
    this.spinner.show();
    this._service.getAllReason(pagNumber, this.reasonCode, this.reasonName, this.trans_toHP).subscribe(res => {
      this.listReason = res.result;
      console.log("listReason", this.listReason);
      this.pagination = res.pagination;
      this.spinner.hide();
    }, error => {
      this._alert.error("Have Error");
      this.spinner.hide();
    });
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.getAllReason(this.pagination.currentPage)
  }
  deleteReason(item: SettingReason) {
    this._alert.confirm('Delete Reason', 'Are you sure you want to delete this setting reason  ?', () => {
      this._service.deleteReason(item).subscribe(() => {
        this._alert.success("Delete setting Reason success")
        this.getAllReason();
      }, error => {
        this._alert.error("Delete setting Reason error");
      })
    });
  }

  addOrEdit(item?: SettingReason, check?: boolean) {
    var data = {
      item,
      check
    }
    this._service.dataAddandEditMail.next(data);
    this._router.navigateByUrl("/rack/setting-reason/add-reason")
  }
  search() {
    this.getAllReason();
  }
  clearSearch() {
    this.reasonCode = "";
    this.trans_toHP = "all";
    this.reasonName = "";
    this.getAllReason();
  }
  getReasonCode() {
    this._service.getReasonCode().subscribe(res => {
      this.listReasonCode = res.map(item => {
        return { id: item, text: item };
      })
    })
  }
}
