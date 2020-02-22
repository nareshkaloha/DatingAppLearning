import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { AlertifyService } from './../../_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from './../../_models/user';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})

export class MemberEditComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private authService: AuthService,
    private userService: UserService,
    private router: Router
    ) { }
  @ViewChild('frmUserEdit', {static: false}) frmUserEdit: NgForm;
  user: User;

  ngOnInit() {
    this.route.data.subscribe( d => {
      this.user = d['user'];
    });
  }

  updateUser() {
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(resp => {
      this.alertify.success('profile updated successfully ..');
      this.frmUserEdit.reset(this.user);
      this.router.navigate(['/home']);
      //console.log(this.frmUserEdit);
    }, err => {
      this.alertify.error(err);
    });
  }
}
