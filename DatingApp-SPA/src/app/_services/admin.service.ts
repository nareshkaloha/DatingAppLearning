import { User } from 'src/app/_models/user';
import { HttpClient } from '@angular/common/http';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  getUsers() {
    return this.httpClient.get(this.baseUrl + 'admin/userWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.httpClient.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }

  getPhotosForModeration() {
    return this.httpClient.get(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(id: number) {
    return this.httpClient.post(this.baseUrl + 'admin/approvePhoto/' + id, {});
  }
}
