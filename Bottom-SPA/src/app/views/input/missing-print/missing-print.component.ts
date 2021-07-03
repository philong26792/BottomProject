import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InputService } from '../../../_core/_services/input.service';
import { TransferDetail } from '../../../_core/_models/transfer-detail';
import { MissingAgain } from '../../../_core/_models/missing-again';

@Component({
  selector: 'app-missing-print',
  templateUrl: './missing-print.component.html',
  styleUrls: ['./missing-print.component.scss']
})
export class MissingPrintComponent implements OnInit {
  elementType: 'url' | 'canvas' | 'img' = 'url';
  today = new Date();
  missingPrint: any = [];
  transferDetailByMissing: TransferDetail[] = [];
  totalQty = 0;
  blanceQty = 0;
  missingNo: string = '';
  checkBackMiss: string = '';
  missingOfBatch: MissingAgain;
  constructor(private router: Router, private inputService: InputService) { }

  ngOnInit() {
    this.inputService.currentMissingPrint.subscribe(res => this.missingOfBatch = res);
    this.getMissingPrint();
    this.inputService.currentCheckBackMissSource.subscribe(res => this.checkBackMiss = res);
  }

  print(e) {
    e.preventDefault();
    const printContents = document.getElementById('wrap-print').innerHTML;
    const originalContents = document.body.innerHTML;
    document.body.innerHTML = printContents;
    window.print();
    document.body.innerHTML = originalContents;
  }

  back() {
    if(this.checkBackMiss === '0') {
      this.router.navigate(['/input/missing-again']);
    } else if(this.checkBackMiss === '1') {
      this.router.navigate(['/input/main']);
    } else {
      
    }
  }

  getMissingPrint() {
    let param = {
      missing_No: this.missingOfBatch.missing_No,
      mO_Seq: this.missingOfBatch.mO_Seq
    }
    this.inputService.printMissing(param).subscribe(res => {
      this.missingPrint = res.materialMissing;
      this.transferDetailByMissing = res.transactionDetailByMissingNo;

      this.totalQty = this.transferDetailByMissing.reduce((transQty, i) => {
        return transQty += i.trans_Qty;
      }, 0);
      this.blanceQty = this.transferDetailByMissing.reduce((untransacQty, i) => {
        return untransacQty += i.untransac_Qty;
      }, 0);
    });
  }

}
