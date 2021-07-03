import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { KanBanRackDetail } from '../../../../_core/_viewmodels/kanban-rack-detail';
import { KanbanByrackService } from '../../../../_core/_services/kanban-byrack.service';
import { KanbanByRackDetail } from '../../../../_core/_models/kanban-by-rack-detail';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})
export class DetailComponent implements OnInit {
  time: Date = new Date();
  constructor(private router: Router,
              private route: ActivatedRoute,
              private rackService: KanbanByrackService, private spinner: NgxSpinnerService) { }
  buildId: string = '';
  code_Ename:string ='';
  rackDetailT2s: KanBanRackDetail[] = [];
  kanbanByRackDetail :KanbanByRackDetail[];
  vancant: number = 0;
  totalItem: number = 0;
  ngOnInit() {
    setInterval(() => {
      this.time = new Date();
    }, 1000);
    this.buildId = this.route.snapshot.params['codeId'];
    this.getData();
  }
  back(){
    this.router.navigate(['kanban/by-rack/main']);
  }
  getData() {
    this.spinner.show();
    this.rackService.getKanbanByRackAreaDetail(this.buildId).subscribe(res=>
      {
        this.kanbanByRackDetail =res;
        this.kanbanByRackDetail.forEach(element => {
          this.vancant += element.rackDetail.filter(item => item.count === 0).length;
          this.totalItem += element.rackDetail.length;
        });
        this.spinner.hide();
      });
  }
  getRackDetail(rack: string, t3: string) {
    if(t3 =="N")
    {
      this.router.navigate(['/kanban/by-rack/po-list/' + this.buildId +'/' + this.code_Ename + '/' + rack]);
    }
    else{
      this.rackService.changeRack(rack);
      this.router.navigate(["/kanban/by-rack/po-list-t3"]);
    }

  }

  exportExcel() {
    this.rackService.exportExcelKanbanByRackDetail(this.buildId);
  }
}
