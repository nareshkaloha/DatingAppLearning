import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  private test = 'test';
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe( res => this.photoUrl = res);
  }

  log(x) {
    console.log(x);
  }

  submit(frm) {
    console.log(frm);

    const userCreden: any = {username: frm.value.username, password: frm.value.password};

    this.authService.login(userCreden)
      .subscribe(res => {
        this.alertify.success('logged in successfully ');
      }, err => {
        this.alertify.error(err);
      }, () => {
        this.router.navigate(['/members']);
      }
      );
  }

  isLoggedIn() {
    // const token = localStorage.getItem('token');
    // console.log('navbar comp token \n' + token);
    // return !!token;
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.currentUser = null;
    this.authService.decodedToken = null;
    this.alertify.message('logged out successfully ..');
    this.router.navigate(['/home']);
  }
}
