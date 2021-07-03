import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ModifyQrCodeAfterSaveModel } from '../../../_core/_models/modify-qrcode-after-save-model';
import { TransferDetail } from '../../../_core/_models/transfer-detail';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';

@Component({
  selector: 'app-detail-qrcode',
  templateUrl: './detail-qrcode.component.html',
  styleUrls: ['./detail-qrcode.component.css']
})
export class DetailQrcodeComponent implements OnInit {
  qrCodeModel: ModifyQrCodeAfterSaveModel;
  detailQrCode: TransferDetail[] = [];
  constructor(private router: Router,
    private modifyQrCodeService: ModifyQrcodeService) { }

  ngOnInit(): void {
    this.modifyQrCodeService.currentQrCodeModel.subscribe(res => {
      if (res == null) {
        this.qrCodeModel = {
          transac_No: '',
          mO_No: '',
          mO_Seq: '',
          material_ID: '',
          material_Name: '',
          actual_Qty: 0,
          modify_Qty: 0,
          modify_Time: new Date(),
          qrCode_ID: '',
          qrCode_Version: 0,
          update_By: '',
          missing_No: ''
        }
      } else {
        this.qrCodeModel = res;
      }
    });
    this.getDetail();
  }
  getDetail() {
    this.modifyQrCodeService.getDetailQrCode(this.qrCodeModel).subscribe(res => {
      this.detailQrCode = res;
      // this.instockQtyTotal = this.detailQrCode.map(x => x.instock_Qty).reduce((a, c) => { return a + c });
    })
  }
  back() {
    this.router.navigate(['/modify-store/list-qrcode-change']);
  }
}
