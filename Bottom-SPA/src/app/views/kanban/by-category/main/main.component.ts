import { Component, OnInit } from '@angular/core';
import { KanbanService } from '../../../../_core/_services/kanban.service';
import { Code } from '../../../../_core/_models/code';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {
  categories: Code[] = [];
  time: Date = new Date();

  constructor(private kanbanService: KanbanService, private router: Router, private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.getData();
    setInterval(() => {
      this.time = new Date();
    }, 1000);
  }

  getData() {
    this.spinner.show();
    this.kanbanService.getKanbanByCategory().subscribe(res => {
      this.categories = res;
      this.spinner.hide();
    });
  }

  detail(codeId: string, codeName: string) {
    this.kanbanService.changeCodeName(codeName);
    this.router.navigate(['/kanban/by-category/detail/' + codeId]);
  }
}
