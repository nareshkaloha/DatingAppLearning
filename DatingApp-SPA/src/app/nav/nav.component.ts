import { AuthService } from './../_services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  private test = 'test';

  constructor(private authService: AuthService) {}

  ngOnInit() {
  }

  log(x) {
    console.log(x);
  }

  submit(frm) {
    console.log(frm);

    const userCreden: any = {username: frm.value.username, password: frm.value.password};

    this.authService.login(userCreden).subscribe(res => {console.log('success'); }, err => {console.log('failure ..'); });
  }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout() {
    localStorage.removeItem('token');
    console.log('logged out');
  }
}
