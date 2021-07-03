import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReceiveAfterSubmit } from '../../../_core/_models/receive-after-submit';
import { MaterialService } from '../../../_core/_services/material.service';
import { ReceivingService } from '../../../_core/_services/receiving.service';

@Component({
  selector: 'app-record',
  templateUrl: './record.component.html',
  styleUrls: ['./record.component.css']
})
export class RecordComponent implements OnInit {
  receives: ReceiveAfterSubmit[];
  constructor(private router: Router,
              private recevingService: ReceivingService,
              private materialService: MaterialService) { }

  ngOnInit(): void {
    this.recevingService.currentReceive.subscribe(res => this.receives = res);
  }
  changeFormDetail(item: ReceiveAfterSubmit) {
    this.recevingService.changeReceiveItem(item);
    this.materialService.receiveNoDetails(item.receive_No).subscribe(res => {
      this.recevingService.changeReceiveNoDetail(res);
      this.router.navigate(['/receive/record-detail']);
    })
  }
  backForm() {
    this.router.navigate(['/receive/main']);
  }
}
