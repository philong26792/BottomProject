import { Component, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { QrCodeSplitDetail } from '../../../_core/_models/merge-qrcode/qr-code-split-detail';

@Component({
  selector: 'app-other-split-qrcode-detail-child',
  templateUrl: './other-split-qrcode-detail-child.component.html',
  styleUrls: ['./other-split-qrcode-detail-child.component.scss']
})
export class OtherSplitQrcodeDetailChildComponent implements OnInit {
  transacNo: string = '';
  qrCodeSplitDetail: QrCodeSplitDetail = {
    article: '',
    listSizeAndQty: [],
    mO_No: '',
    material_ID: '',
    material_Name: '',
    model_Name: '',
    model_No: ''
  };

  constructor(private location: Location,
    private mergeQrcodeService: MergeQrcodeService,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.transacNo = this.route.snapshot.params['transacNo'];
    this.mergeQrcodeService.qrCodeSplitDetail(this.transacNo).subscribe(res => {
      this.qrCodeSplitDetail = res;
    });
  }

  back() {
    this.location.back();
  }

}
