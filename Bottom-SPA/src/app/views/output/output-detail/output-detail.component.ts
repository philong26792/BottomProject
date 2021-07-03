import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OutputService } from '../../../_core/_services/output.service';

@Component({
  selector: 'app-output-detail',
  templateUrl: './output-detail.component.html',
  styleUrls: ['./output-detail.component.scss'],
})
export class OutputDetailComponent implements OnInit {
  outputDetail: any = [];
  totalTransOutQty: number;
  result1 = [];
  transacNo: string = '';

  constructor(
    private router: Router,
    private outputService: OutputService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.transacNo = this.route.snapshot.params['transacNo'];
    this.getData();
  }

  back() {
    this.router.navigate(['output/main']);
  }
  getData() {
    this.outputService.getOutputDetail(this.transacNo).subscribe(res => {
      this.outputDetail = res;
      this.totalTransOutQty = this.outputDetail.transactionDetail
        .map(x => x.trans_Qty).reduce((a,c) => {return a + c});
      // Group by theo tool_Size
      const groups = new Set(this.outputDetail.transactionDetail.map((item) => item.tool_Size)), results = [];
      groups.forEach((g) =>
        results.push({
          name: g,
          value: this.outputDetail.transactionDetail.filter((i) => i.tool_Size === g).reduce((trans_Qty, j) => {
            return trans_Qty += j.trans_Qty;
          }, 0),
          colspan: this.outputDetail.transactionDetail.filter((i) => i.tool_Size === g).length
        })
      );
      this.result1 = results;
    });
  }
}
