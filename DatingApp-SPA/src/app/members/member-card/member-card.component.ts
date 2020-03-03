import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { User } from './../../_models/user';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input('UserCard') userCard: User;
  loggedInUser: User = JSON.parse(localStorage.getItem('user'));

  constructor(
              private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
  }

  liked() {
    this.userService.likeUser(this.authService.decodedToken.nameid , this.userCard.id).subscribe( res => {
      this.alertify.success(`You liked ${this.userCard.knownAs}`);
    }, err => {
      this.alertify.error(err);
    });
  }
}
