import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService) {}

  canActivate(next: ActivatedRouteSnapshot): boolean {
    //const userRoles = next.firstChild.data['roles'] as Array<string>; // this does not work ..
    const userRoles = next.data['roles'] as Array<string>;
    console.log(userRoles);

    if (userRoles) {
      const allowedRole = this.authService.isInAllowedRole(userRoles);

      if (allowedRole) {
        return true;
      } else {
        this.alertify.error('You are not authorized to access this page ..');
        this.router.navigate(['members']);
      }
    }

    if (this.authService.loggedIn()) {
      return true;
    }

    this.alertify.error('Please login first ..');
    this.router.navigate(['/home']);
    return false;
  }
}
