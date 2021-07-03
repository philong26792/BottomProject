import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { MissingPrint } from '../../../_core/_models/missing-print';
import { InputService } from '../../../_core/_services/input.service';
import { ModifyQrcodeService } from '../../../_core/_services/modify-qrcode.service';

@Component({
  selector: 'app-print-list-missing',
  templateUrl: './print-list-missing.component.html',
  styleUrls: ['./print-list-missing.component.scss']
})
export class PrintListMissingComponent implements OnInit {
  elementType: 'url' | 'canvas' | 'img' = 'url';
  today = new Date();
  listMissingPrint: MissingPrint[] = [];
  listMissingNo: string[] = [];

  constructor(private router: Router,
    private inputService: InputService,
    private modifyQrCode: ModifyQrcodeService,
    private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.modifyQrCode.currentlistMissingPrint.subscribe(res => {
      this.listMissingNo = res;
    }).unsubscribe();
    this.getMissingPrint();
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
    this.router.navigate(['/stock-adj/list-qrcode-change']);
  }

  getMissingPrint() {
    this.spinner.show();
    this.inputService.printListMissing(this.listMissingNo).subscribe(res => {
      this.listMissingPrint = res;
      this.spinner.hide();
    }, error => {
      console.log(error);
      this.spinner.hide();
    });
  }

}
