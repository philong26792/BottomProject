import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReceivingService } from '../../../_core/_services/receiving.service';
import { ReceiveNoDetail } from '../../../_core/_viewmodels/receive-no-detail';

@Component({
  selector: 'app-record-detail',
  templateUrl: './record-detail.component.html',
  styleUrls: ['./record-detail.component.css']
})
export class RecordDetailComponent implements OnInit {
  receiveNoDetail: ReceiveNoDetail[] = [];
  receiveItemModel: any;
  totalPurchaseQty: number;
  totalReceivedQty: number;
  constructor(private router: Router,
              private receivingService: ReceivingService) { }

  ngOnInit(): void {
    this.receivingService.currentReceiveitem.subscribe(res => this.receiveItemModel = res);
    this.receivingService.currentReceiveNoDetail.subscribe(res => this.receiveNoDetail = res);
    if(this.receiveNoDetail.length > 0) {
      this.totalPurchaseQty = this.receiveNoDetail.map(x => x.purchase_Qty).reduce((a,c) => {return a + c});
      this.totalReceivedQty = this.receiveNoDetail.map(x => x.received_Qty).reduce((a,c) => {return a + c});
    }
  }
  backForm() {
    this.router.navigate(['/receive/record/']);
  }
}
