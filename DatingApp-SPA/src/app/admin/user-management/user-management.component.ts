import { RolesModalComponent } from './../roles-modal/roles-modal.component';
import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  roles: string[];
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService,
              private alertify: AlertifyService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

 getUsersWithRoles() {
  this.adminService.getUsers().subscribe((resp: User[]) => {
    this.users = resp;
  }, err => {
    this.alertify.error(err);
  });
 }

 editRoles(user: User) {

  const initialState = {
    user,
    roles: this.getRoles(user)
  };
  this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
  //this.bsModalRef.content.closeBtnName = 'Close'; 
  this.bsModalRef.content.updatedRoles.subscribe(resp => {
    //console.log(resp);
    const rolesToUpdate = {
      userroles: [...resp.filter(el => el.checked === true).map(el => el.name)]
    };

    //console.log(rolesToUpdate);
    this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(resp => {
      user.roles = [...rolesToUpdate.userroles];
    }, err => {
      this.alertify.error(err);
    })
  });
 }

 getRoles(user: User) {
   const roles: any[] = [];
   const userRoles = user.roles;
   const availabeRoles: any[] = [
    {name: 'Admin', value: 'Admin'},
    {name: 'Member', value: 'Member'},
    {name: 'Moderator', value: 'Moderator'},
    {name: 'VIP', value: 'VIP'}
   ];

   for (let i = 0; i < availabeRoles.length; i++) {
     let isMatch = false;
     for (let j = 0; j < userRoles.length; j++) {
       if (availabeRoles[i].name === userRoles[j]) {
         isMatch = true;
         roles.push({name: availabeRoles[i].name, value: availabeRoles[i].value, checked: true});
         break;
       }
     }

     if (!isMatch) {
      roles.push({name: availabeRoles[i].name, value: availabeRoles[i].value, checked: false});
     }

   }
   return roles;
 }

}
