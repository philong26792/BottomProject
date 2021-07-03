import { Injectable } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { CanActivate, Router } from '@angular/router';
import { commonPerFactory } from '../_utility/common-per-factory';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
  urlBackNotLogin = commonPerFactory.urlBackNotLogin;
  constructor(private authService: AuthService, private router: Router) { }
  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }
    else {
      window.location.href = this.urlBackNotLogin;
    }
  }
}
