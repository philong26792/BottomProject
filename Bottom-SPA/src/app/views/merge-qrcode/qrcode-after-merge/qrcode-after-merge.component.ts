import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { OutputService } from '../../../_core/_services/output.service';
import { QrCodeAfterMerge } from '../../../_core/_viewmodels/merge-qrcode/qrCode-after-merge';

@Component({
  selector: 'app-qrcode-after-merge',
  templateUrl: './qrcode-after-merge.component.html',
  styleUrls: ['./qrcode-after-merge.component.css']
})
export class QrcodeAfterMergeComponent implements OnInit {
  qrCodeMergeModelList: QrCodeAfterMerge[];
  constructor(private qrCodeMergeService: MergeQrcodeService,
              private outputService: OutputService,
              private router: Router) { }
  ngOnInit(): void {
    this.qrCodeMergeService.currentListQrCodeAfterMerge.subscribe(res => this.qrCodeMergeModelList = res);
    if(this.qrCodeMergeModelList.length === 0) {
      this.router.navigate(['/merge-qrcode/main']);
    }
  }
  showDetail(data: QrCodeAfterMerge) {
    this.qrCodeMergeService.changeQrCodeAfterMerge(data);
    this.router.navigate(['/merge-qrcode/qrcode-detail']);
  }
  print(data: QrCodeAfterMerge) {
    this.outputService.changePrintQrCode('mergeQrcode');
    const paramPrintQrCodeAgain = [{
      qrCode_ID: data.qrCode_ID,
      qrCode_Version: 1,
      mO_Seq: ''
    }];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
    // this.qrCodeMergeService.changeQrCodeAfterMerge(data);
    // this.router.navigate(['/merge-qrcode/print']);
  }
}
