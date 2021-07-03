import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { RackArea } from "../../../../_core/_models/rack-area";
import { KanbanByrackService } from "../../../../_core/_services/kanban-byrack.service";
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: "app-main",
  templateUrl: "./main.component.html",
  styleUrls: ["./main.component.scss"],
})
export class MainComponent implements OnInit {
  racks: RackArea[] = [];
  time: Date = new Date();
  constructor(
    private kanbanService: KanbanByrackService,
    private router: Router,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit() {
    this.getData();
    setInterval(() => {
      this.time = new Date();
    }, 1000);
  }
  getData() {
    this.spinner.show();
    this.kanbanService.getKanbanByRackArea().subscribe((res) => {
      this.racks = res;
      this.spinner.hide();
    });
  }

  detail(codeId: string, code_Ename: string) {
    this.router.navigate([
      "/kanban/by-rack/detail/" + codeId ]);
  }
}
