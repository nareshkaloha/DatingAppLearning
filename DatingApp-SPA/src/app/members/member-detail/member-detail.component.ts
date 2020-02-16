import { Photo } from './../../_models/photo';
import { User } from './../../_models/user';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private userService: UserService, private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    //this.loadUser();
    this.route.data.subscribe(d => {
      this.user = d['user'];
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        preview: false,
        imageAnimation: NgxGalleryAnimation.Slide
      }
    ];

    this.galleryImages = this.getImages();
  }

  // this is no longer needed ..
  loadUser() {
    this.userService.getUser(+this.route.snapshot.params['id']).subscribe((resp: User) => {
      this.user = resp;
    }, error => {
      this.alertify.error(error);
    });
  }

  getImages() {
    const imageUrls = [];

    for (const image of this.user.photos) {
      imageUrls.push( {
        small: image.url,
        medium: image.url,
        big: image.url
      });
    }

    return imageUrls;
  }
}
