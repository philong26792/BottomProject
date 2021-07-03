import { Component, OnInit } from '@angular/core';
import { WMSB_Transaction_Detail } from '../../../_core/_models/transaction-detail';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { QrCodeAfterMerge } from '../../../_core/_viewmodels/merge-qrcode/qrCode-after-merge';
import {Location} from '@angular/common';

@Component({
  selector: 'app-qrcode-detail',
  templateUrl: './qrcode-detail.component.html',
  styleUrls: ['./qrcode-detail.component.css']
})
export class QrcodeDetailComponent implements OnInit {
  qrCodeAfterMerge: QrCodeAfterMerge;
  qrCodeDetail: WMSB_Transaction_Detail[] = [];
  constructor(private mergeQrCodeService: MergeQrcodeService,
              private alertifyService: AlertifyService,
              private location: Location) { }

  ngOnInit(): void {
    this.mergeQrCodeService.currentQrCodeAfterMerge.subscribe(res => this.qrCodeAfterMerge = res).unsubscribe();
    this.getQrCodeDetail();
  }
  getQrCodeDetail() {
    this.mergeQrCodeService.getQrCodeDetail(this.qrCodeAfterMerge).subscribe(res => {
      this.qrCodeDetail = res;
    }, error => {
      this.alertifyService.error(error);
    });
  }

  back() {
    this.location.back();
  }
}
