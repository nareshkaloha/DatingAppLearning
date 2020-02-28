import { Pagination } from './../../_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { User } from '../../_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  loggedInUser: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'males'}, {value: 'female', display: 'females'}];
  userParams: any = {};

  constructor(private userService: UserService,
              private alertify: AlertifyService,
              private route: ActivatedRoute
              ) { }

  ngOnInit() {
    //this.loadUsers();
    this.route.data.subscribe(d => {
      this.users = d['users'].result;
      this.pagination = d['users'].pagination;
      //console.log(this.pagination);

      this.userParams.gender = this.loggedInUser.gender === 'male' ? 'female' : 'male';
      this.userParams.minAge = 18;
      this.userParams.maxAge = 99;
      this.userParams.orderBy = 'lastActiveDate';
    });
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    console.log(this.pagination.currentPage);
    this.loadUsers();
  }

  resetFilter() {
    this.userParams.gender = this.loggedInUser.gender === 'male' ? 'female' : 'male';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUsers();
  }

  loadUsers() {
     this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
       .subscribe( resp => {
         this.users = resp.result;
         this.pagination = resp.pagination;
       }, err => {
         this.alertify.error(err);
       });
   }
}
