import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseURL: string =  environment.apiUrl + 'auth/';
  decodedToken: any;
  private jwtDecode = new JwtHelperService();

  constructor(private http: HttpClient) { }

  login(userCredentials: any) {  
    return this.http.post(this.baseURL + 'login', userCredentials).pipe(
      map( (resp: any) => {
        const user = resp;
        if(user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = this.jwtDecode.decodeToken(user.token);
          //console.log(user.token);          
        }        
      })
    );
  }

  register(user: any) {
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
}
