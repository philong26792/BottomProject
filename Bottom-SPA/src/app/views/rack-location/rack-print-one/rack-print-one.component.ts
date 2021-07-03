import { Component, OnInit } from '@angular/core';
import { RackService } from '../../../_core/_services/rack.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-rack-print-one',
  templateUrl: './rack-print-one.component.html',
  styleUrls: ['./rack-print-one.component.scss']
})
export class RackPrintOneComponent implements OnInit {
  rackItem: any = [];
  constructor(private rackServcie: RackService, private router: Router) { }

  ngOnInit() {
    this.rackServcie.currentArr.subscribe(rackItem => this.rackItem = rackItem)

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
    this.router.navigate(['/rack/main']);
  }
}
