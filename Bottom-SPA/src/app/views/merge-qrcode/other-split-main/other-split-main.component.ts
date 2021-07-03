import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { MergeQrCodeModel } from '../../../_core/_viewmodels/merge-qrcode/merge-qrcode-model';

@Component({
  selector: 'app-other-split-main',
  templateUrl: './other-split-main.component.html',
  styleUrls: ['./other-split-main.component.scss']
})
export class OtherSplitMainComponent implements OnInit {
  listSplitTransaction: MergeQrCodeModel[] = [];
  qrCodeID: string = '';
  moNo: string = '';

  constructor(private mergeQrcodeService: MergeQrcodeService,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private router: Router,
    private functionUtility: FunctionUtility,
    private modifyQrCodeService: ModifyQrcodeService) { }

  ngOnInit(): void {
    this.mergeQrcodeService.currentparamSearchOtherSplitMain.subscribe(res => {
      this.moNo = res.moNo;
      this.qrCodeID = res.qrCodeID;
    }).unsubscribe();
    if (this.moNo != null) {
      this.getData();
    }
  }

  getData() {
    if (this.functionUtility.checkEmpty(this.moNo)) {
      this.alertify.error('Please choose Plan No');
      return;
    }
    
    this.spinner.show();
    this.mergeQrcodeService.searchOtherSplitMain(this.moNo, this.qrCodeID).subscribe(res => {
      this.listSplitTransaction = res;
      if (res.length == 0) {
        this.alertify.error('No Data!');
      }
      this.spinner.hide();
    }, error => {
      this.spinner.hide();
    });
  }

  search() {
    this.getData();
  }

  gotoDetail(transacNo: string) {
    const paramSearch = {
      moNo: this.moNo,
      qrCodeID: this.qrCodeID
    };
    this.mergeQrcodeService.changeParamSearchOtherSplitMain(paramSearch);
    this.router.navigate(['/merge-qrcode/other-split/detail/', transacNo]);
  }

  getPlanNoByQrCodeId() {
    if (this.qrCodeID.length == 15) {
      this.modifyQrCodeService.getPlanNoByQrCodeId(this.qrCodeID).subscribe(res => {
        this.moNo = res.planNo;
      });
    }
  }
}
