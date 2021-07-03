import { Component, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { KanbanByPoDetailByReceivingType } from '../../../../_core/_models/kanban-by-po-detail-by-receiving-type';
import { KanbanByPoService } from '../../../../_core/_services/kanban-by-po.service';

@Component({
  selector: 'app-detail-by-receiving-type',
  templateUrl: './detail-by-receiving-type.component.html',
  styleUrls: ['./detail-by-receiving-type.component.scss']
})
export class DetailByReceivingTypeComponent implements OnInit {
  listKanbanByPoDetailByReceivingType: KanbanByPoDetailByReceivingType[] = [];
  kanbanByPoDetail: any = [];
  kanbanByPo: any = [];
  time: Date = new Date();
  nodata: boolean = false;
  kanbanByPoDetailByReceivingType: KanbanByPoDetailByReceivingType = {
    article: '',
    line_ID: '',
    mO_No: '',
    mO_Seq: '',
    material_Name: '',
    material_No: '',
    model_Name: '',
    plan_Qty: 0,
    plan_Start_STF: '',
    qrCode_ID: '',
    rack_Location: '',
    receiving_Type: '',
    t2_Supplier: '',
    total_Pairs: 0,
    transaction_Date: ''
  }
  constructor(private kanbanByPoService: KanbanByPoService,
    private spinner: NgxSpinnerService) { }

  ngOnInit() {
    setInterval(() => {
      this.time = new Date();
    }, 1000);

    this.kanbanByPoService.currentKanbanByPo.subscribe(res => {
      this.kanbanByPo = res;
    }).unsubscribe();

    this.kanbanByPoService.currentKanbanByPoDetail.subscribe(res => {
      this.kanbanByPoDetail = res;
    }).unsubscribe();
    this.getData();
  }

  getData() {
    this.spinner.show();
    this.kanbanByPoService.getDetailKanbanByPoByReceivingType(this.kanbanByPoDetail.mO_No, this.kanbanByPoDetail.mO_Seq, this.kanbanByPoDetail.material_NO).subscribe(res => {
      this.listKanbanByPoDetailByReceivingType = res;
      if (this.listKanbanByPoDetailByReceivingType.length > 0) {
        this.kanbanByPoDetailByReceivingType = this.listKanbanByPoDetailByReceivingType[0];
      }
      else {
        this.nodata = true;
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
    });
  }

}
