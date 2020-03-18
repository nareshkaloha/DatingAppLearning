import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { environment } from './../../../environments/environment';
import { Photo } from './../../_models/photo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() changedMainPhoto = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;
  photUrl: string;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService) {
  }

  ngOnInit() {
    //this.initiliazedFileUploader();
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos', // we need the user id
      authToken: 'Bearer ' + localStorage.getItem('token'), // we need to pass the token
      isHTML5: true,
      allowedMimeType: ['image/jpeg', 'images', 'png', 'jpg'],
      maxFileSize: 10 * 1024 * 1024, // it will make the maximum file size of 10mbs
      autoUpload: false, // click a button in order to send this up
      removeAfterUpload: true // after the photo is being uploaded we want to remove it from the upload queue
    });
    this.uploader.onAfterAddingFile = file => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          isMain: res.isMain,
          description: res.description,
          createdDate: res.createdDate,
          isApproved: null
        };

        this.photos.push(photo);

        if(photo.isMain) {
          this.authService.changeMainPhoto(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe( resp => {
      //this.alertify.success('set as main photo');
      this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
      this.currentMainPhoto.isMain = false;
      photo.isMain = true;
      this.changedMainPhoto.emit(photo.url);
      this.authService.changeMainPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    },
    err => {
      this.alertify.error(err);
    }
    );
  }

  deletePhoto(id: number) {
    this.alertify.confirm('are you sure you want to delete this photo', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe( resp => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('photo deleted');
      }, err => {
        this.alertify.error(err);
      });
    });
  }
}
