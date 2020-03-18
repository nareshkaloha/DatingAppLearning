import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';
import { AdminService } from 'src/app/_services/admin.service';
import { Photo } from './../../_models/photo';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[];

  constructor(private adminService: AdminService,
              private alertify: AlertifyService,
              private userService: UserService,
              private authService: AuthService) { }

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe( (resp: Photo[]) => {
      this.photos = resp;
    }, err => {
      this.alertify.error(err);
    });
  }

  approvePhoto(photo: Photo) {
    this.adminService.approvePhoto(photo.id).subscribe(resp => {
      const index = this.photos.indexOf(photo);
      if (index > -1) {
        this.photos.splice(index, 1);
      }
      this.alertify.success('photo approved');
    }, err => {
      this.alertify.error(err);
    });
  }

  rejectPhoto(photo: Photo) {
    this.userService.deletePhoto(this.authService.decodedToken.nameid, photo.id).subscribe(resp => {
      const index = this.photos.indexOf(photo);
      if (index > -1) {
        this.photos.splice(index, 1);
      }
      this.alertify.success('photo rejected');
    }, err => {
      this.alertify.error(err);
    });
  }

}
