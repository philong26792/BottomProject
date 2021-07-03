import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ModifyQrCodeAfterSaveModel } from '../../../_core/_models/modify-qrcode-after-save-model';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';
import { OutputService } from '../../../_core/_services/output.service';

@Component({
  selector: 'app-list-qrcode-change',
  templateUrl: './list-qrcode-change.component.html',
  styleUrls: ['./list-qrcode-change.component.css']
})
export class ListQrcodeChangeComponent implements OnInit {
  modifyQrCodeList: ModifyQrCodeAfterSaveModel[] = [];
  otherType = '';
  constructor(private modifyQrCodeService: ModifyQrcodeService,
              private outputService: OutputService,
              private router: Router) { }

  ngOnInit(): void {
    this.modifyQrCodeService.currentOtherType.subscribe(res => this.otherType = res);
    this.modifyQrCodeService.currentmodifyQrCodeAfterSave.subscribe(res => {
      if (res == null) {
        this.modifyQrCodeList = [];
      } else {
        this.modifyQrCodeList = res;
        console.log(res);
      }
    });
  }
  detailQrCode(model: ModifyQrCodeAfterSaveModel) {
    this.modifyQrCodeService.changeQrCodeModel(model);
    this.router.navigate(['/modify-store/detail-qrcode']);
  }
  printQrCode(model: ModifyQrCodeAfterSaveModel) {
    const param = [
      {
        qrCode_ID: model.qrCode_ID,
        qrCode_Version: model.qrCode_Version,
        mO_Seq: model.mO_Seq,
      },
    ];
    this.outputService.changePrintQrCode("5");
    this.outputService.changeParamPrintQrCodeAgain(param);
    this.router.navigate(["/output/print-qrcode-again"]);
  }
  cancel() {

  }
  printAll() {
    this.outputService.changePrintQrCode("5");
    let param = this.modifyQrCodeList.map(obj => {
      return { 
        qrCode_ID: obj.qrCode_ID,
        qrCode_Version: obj.qrCode_Version,
        mO_Seq: obj.mO_Seq,
      };
    })
    this.outputService.changeParamPrintQrCodeAgain(param);
    this.router.navigate(["/output/print-qrcode-again"]);
  }
  printMissing() {
    let param = this.modifyQrCodeList.map(item => {
      return item.missing_No;
    })
    this.modifyQrCodeService.changeListMissingPrint(param);
    this.router.navigate(["/modify-store/print-list-missing"]);
  }
}
