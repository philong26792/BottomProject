import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReceiveNoDetail } from '../../../_core/_viewmodels/receive-no-detail';
import { MaterialService } from '../../../_core/_services/material.service';
import { ReceiveNoMain } from '../../../_core/_viewmodels/receive_no_main';

@Component({
  selector: 'app-record-detail',
  templateUrl: './record-detail.component.html',
  styleUrls: ['./record-detail.component.scss']
})
export class RecordDetailComponent implements OnInit {
  receiveDetail: ReceiveNoDetail[] = [];
  receiveNoMain: ReceiveNoMain;
  totalPurchaseQty: number;
  totalReceiveQty: number;
  constructor(private router: Router,
              private materialService: MaterialService) { }

  ngOnInit() {
    this.materialService.currentReceiveNoMainItem.subscribe(res => this.receiveNoMain = res);
    this.materialService.currentReceiveNoDetail.subscribe(res => this.receiveDetail = res );
    if(this.receiveDetail.length > 0) {
      this.totalPurchaseQty = this.receiveDetail.map(x => x.purchase_Qty).reduce((a,c) => {return a + c});
      this.totalReceiveQty = this.receiveDetail.map(x => x.received_Qty).reduce((a,c) => {return a + c});
    }
  }
  backForm() {
    this.router.navigate(['receipt/record/']);
  }
}
