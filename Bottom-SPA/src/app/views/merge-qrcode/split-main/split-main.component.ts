import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { AlertifyService } from '../../../_core/_services/alertify.service';
import { MergeQrcodeService } from '../../../_core/_services/merge-qrcode.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';
import { FunctionUtility } from '../../../_core/_utility/function-utility';
import { MergeQrCodeModel } from '../../../_core/_viewmodels/merge-qrcode/merge-qrcode-model';

@Component({
  selector: 'app-split-main',
  templateUrl: './split-main.component.html',
  styleUrls: ['./split-main.component.scss']
})
export class SplitMainComponent implements OnInit {
  listSplitTransaction: MergeQrCodeModel[] =  [];
  qrCodeID: string = '';
  moNo: string = '';
  searchByPrebook: boolean = true;
  timeFrom: Date = null;
  timeEnd: Date = null;

  constructor(private mergeQrcodeService: MergeQrcodeService,
    private spinner: NgxSpinnerService,
    private alertify: AlertifyService,
    private router: Router,
    private functionUtility: FunctionUtility,
    private modifyQrCodeService: ModifyQrcodeService) { }

  ngOnInit(): void {
    this.mergeQrcodeService.currentparamSearchSplitMain.subscribe(res => {
      this.moNo = res.moNo;
      this.qrCodeID = res.qrCodeID;
    }).unsubscribe();
    if (this.moNo != '') {
      this.getData();
    }
  }

  getData() {
    if (!this.searchByPrebook && this.functionUtility.checkEmpty(this.moNo)) {
      this.alertify.error('Please choose Plan No');
      return;
    }
    this.spinner.show();
    this.mergeQrcodeService.searchSplitMain(this.moNo, this.qrCodeID, this.timeFrom, this.timeEnd, this.searchByPrebook).subscribe(res => {
      this.listSplitTransaction = res;
      if (res.length == 0) {
        this.alertify.error('No Data!');
      }
      this.spinner.hide();
    }, error => {
      this.alertify.error(error);
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
    this.mergeQrcodeService.changeParamSearchSplitMain(paramSearch);
    this.router.navigate(['/merge-qrcode/split/detail/', transacNo]);
  }

  getPlanNoByQrCodeId() {
    if (this.qrCodeID.length == 15) {
      this.modifyQrCodeService.getPlanNoByQrCodeId(this.qrCodeID).subscribe(res => {
        this.moNo = res.planNo;
      });
    }
  }
}
