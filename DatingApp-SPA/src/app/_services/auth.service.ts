import { RouterModule } from '@angular/router';
import { User } from './../_models/user';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseURL: string =  environment.apiUrl + 'auth/';
  decodedToken: any;
  private jwtDecode = new JwtHelperService();
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  changeMainPhoto(photUrl: string) {
    this.photoUrl.next(photUrl);
  }

  login(userCredentials: User) {
    return this.http.post(this.baseURL + 'login', userCredentials).pipe(
      map( (resp: any) => {
        const user = resp;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = this.jwtDecode.decodeToken(user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.currentUser = user.user;
          //console.log(this.userForStorage);
          this.changeMainPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(user: User) {
    return this.http.post(this.baseURL + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    //console.log('token from local storage \n' + token);
    //console.log(this.jwtDecode.decodeToken(token));
    const retValue = !this.jwtDecode.isTokenExpired(token);
    //console.log(retValue)    ;
    return retValue;
  }

  isInAllowedRole(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;

    allowedRoles.forEach(element => {
      if (userRoles.includes(element)) {
        isMatch = true;
        return;
      }
    });

    return isMatch;
  }
}
