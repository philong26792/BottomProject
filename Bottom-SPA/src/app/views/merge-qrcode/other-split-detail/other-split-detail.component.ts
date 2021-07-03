import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { SplitMain, SplitDetail } from '../../../_core/_models/merge-qrcode/split-main';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { OutputService } from '../../../_core/_services/output.service';

@Component({
  selector: 'app-other-split-detail',
  templateUrl: './other-split-detail.component.html',
  styleUrls: ['./other-split-detail.component.scss']
})
export class OtherSplitDetailComponent implements OnInit {
  transacNo: string = '';
  splitInfo: SplitMain = {
    splitPlanNoParent: {
      mO_No: '',
      transac_No: '',
      transac_Type: '',
      mO_Seq: '',
      material_ID: '',
      material_Name: '',
      rack_Location: '',
      split_Time: null,
      preBuy_MO_No: '',
      updated_By: '',
      stock_Qty: 0,
      qrCode_ID: '',
      qrCode_Version: 0
    },
    splitPlanNoChild: []
  };

  constructor(private route: ActivatedRoute,
    private router: Router,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private mergeQrcodeService: MergeQrcodeService,
    private outputService: OutputService) { }

  ngOnInit(): void {
    this.transacNo = this.route.snapshot.params['transacNo'];
    this.getData();
  }

  getData() {
    this.spinner.show();
    this.mergeQrcodeService.searchSplitInfoDetail(this.transacNo).subscribe(res => {
      this.splitInfo = res;
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
    });
  }

  detail(transacNo: string) {
    this.router.navigate(['/merge-qrcode/other-split/qrcode-detail-child', transacNo]);
  }

  print(splitDetail: SplitDetail) {
    this.outputService.changePrintQrCode('7');
    const paramPrintQrCodeAgain = [{
      qrCode_ID: splitDetail.qrCode_ID,
      qrCode_Version: splitDetail.qrCode_Version,
      mO_Seq:  splitDetail.mO_Seq
    }];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
  }

  printParent(splitDetail: SplitDetail) {
    this.outputService.changePrintQrCode('7');
    const paramPrintQrCodeAgain = [{
      qrCode_ID: splitDetail.qrCode_ID,
      qrCode_Version: splitDetail.qrCode_Version + 1,
      mO_Seq:  splitDetail.mO_Seq
    }];
    this.outputService.changeParamPrintQrCodeAgain(paramPrintQrCodeAgain);
    this.router.navigate(['/output/print-qrcode-again']);
  }

}
