import { Component, OnInit } from '@angular/core';
import { navItems, NavItem } from '../../_nav';
import { AuthService } from '../../_core/_services/auth.service';
import { AlertifyService } from '../../_core/_services/alertify.service';
import { Router } from '@angular/router';
import { commonPerFactory } from '../../_core/_utility/common-per-factory';
import { HpUploadService } from '../../_core/_services/hp-upload.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss']
})
export class DefaultLayoutComponent implements OnInit {
  urlBackNotLogin = commonPerFactory.urlBackNotLogin;
  hpUploadTimeLog: any = {};
  color:string ="";
  public sidebarMinimized = false;
  // public navItems = navItems;
  public navItems = [];
  user: any = JSON.parse(localStorage.getItem('user'));

  /**
   *
   */
  constructor(private authService: AuthService,
    private hpUploadTimeService: HpUploadService,
    private alertify: AlertifyService,
    private router: Router,
    private nav: NavItem) {
    this.navItems = this.nav.getNav();
  }
  ngOnInit() {
    this.hpUploadTimeService.getInfomartionHpUpload().subscribe(res => {
      this.hpUploadTimeLog = res.data;
      this.color = res.color;
      console.log(res);
    });
  }
  toggleMinimize(e) {
    this.sidebarMinimized = e;
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.message('Logged out');
    window.location.href = this.urlBackNotLogin;
  }
}
