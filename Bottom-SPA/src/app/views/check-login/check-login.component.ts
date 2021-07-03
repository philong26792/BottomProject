import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../_core/_services/auth.service';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { commonPerFactory } from '../../_core/_utility/common-per-factory';

@Component({
  selector: 'app-check-login',
  templateUrl: './check-login.component.html',
  styleUrls: ['./check-login.component.scss']
})
export class CheckLoginComponent implements OnInit {
  urlBackNotLogin = commonPerFactory.urlBackNotLogin;

  constructor(private authService: AuthService,
    private router: Router,
    private spinner: NgxSpinnerService) {

  }

  async ngOnInit() {
    this.spinner.show();
    const returnUrl = this.router.url;
    if (returnUrl.includes('?account=')) {
      let username = '';
      if (returnUrl.includes('&url=')) {
        const indexUrl = returnUrl.indexOf('&url=');
        username = returnUrl.substring(returnUrl.lastIndexOf('?account=') + 9, indexUrl);
      }
      else {
        username = returnUrl.substring(returnUrl.lastIndexOf('?account=') + 9);
      }
      await this.authService.checkLogin(username).catch(err => {
        this.spinner.hide();
        window.location.href = this.urlBackNotLogin;
      });
    }
    if (this.authService.loggedIn()) {
      this.spinner.hide();
      if (returnUrl.includes('&url=')) {
        const url = returnUrl.substring(returnUrl.lastIndexOf('&url=') + 5).toString();
        // const urlReturn = '/' + url.replace(/[%2F]/g, '/');
        // const urlReturn = '/' + url.replace(/%2F/g, '/');
        // const urlReturn = '/' + url.replaceAll('%2F', '/');
        const urlReturn = '/' + url.split('%2F').join('/');
        this.router.navigate([urlReturn]);
      }
      else {
        this.router.navigate(['/dashboard']);
      }
    }
    else {
      this.spinner.hide();
      window.location.href = this.urlBackNotLogin;
    }
  }
}
