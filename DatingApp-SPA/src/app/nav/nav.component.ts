import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  private test = 'test';

  constructor(public authService: AuthService, private alertify: AlertifyService) {}

  ngOnInit() {
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
      });
  }

  isLoggedIn() {
    // const token = localStorage.getItem('token');
    // console.log('navbar comp token \n' + token);
    // return !!token;
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('logged out successfully ..');
  }
}
