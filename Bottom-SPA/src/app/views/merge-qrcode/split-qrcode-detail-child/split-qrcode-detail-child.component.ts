import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { from } from 'rxjs';
import { QrCodeSplitDetail } from '../../../_core/_models/merge-qrcode/qr-code-split-detail';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-split-qrcode-detail-child',
  templateUrl: './split-qrcode-detail-child.component.html',
  styleUrls: ['./split-qrcode-detail-child.component.scss']
})
export class SplitQrcodeDetailChildComponent implements OnInit {
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

  constructor(private mergeQrcodeService: MergeQrcodeService,
    private location: Location,
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
