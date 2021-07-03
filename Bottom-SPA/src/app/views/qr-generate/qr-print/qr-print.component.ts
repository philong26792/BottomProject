import { Component, OnInit } from '@angular/core';
import { PackingPrintAll } from '../../../_core/_viewmodels/packing-print-all';
import { Router } from '@angular/router';
import { MaterialFormService } from '../../../_core/_services/material-form.service';

@Component({
  selector: 'app-qr-print',
  templateUrl: './qr-print.component.html',
  styleUrls: ['./qr-print.component.scss']
})
export class QrPrintComponent implements OnInit {
  suggested: string;
  elementType: 'url' | 'canvas' | 'img' = 'url';
  packingPrint: PackingPrintAll[] = [];
  constructor(private materialFormService: MaterialFormService,
              private router: Router) { }

  ngOnInit() {
    this.materialFormService.currentPackingPrint.subscribe(res => this.packingPrint = res);
    this.suggested = localStorage.getItem('suggested');
  }
  trackByFn(index: number, item: PackingPrintAll): string {
    return item.qrCodeMainItem.qrCode_ID;
  }
  back() {
    this.router.navigate(['/qr/body']);
  }
}
