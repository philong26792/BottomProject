import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { QrCodeSplitDetail } from '../../../_core/_models/merge-qrcode/qr-code-split-detail';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';

@Component({
  selector: 'app-other-split-qrcode-detail',
  templateUrl: './other-split-qrcode-detail.component.html',
  styleUrls: ['./other-split-qrcode-detail.component.scss']
})
export class OtherSplitQrcodeDetailComponent implements OnInit {
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
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.transacNo = this.route.snapshot.params['transacNo'];
    this.mergeQrcodeService.qrCodeSplitDetail(this.transacNo).subscribe(res => {
      this.qrCodeSplitDetail = res;
    });
  }

  back() {
    this.router.navigate(['/merge-qrcode/other-split/detail', this.transacNo])
  }

}
