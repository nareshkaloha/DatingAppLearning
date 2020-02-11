import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseURL: string = "http://localhost:5000/api/auth/";

  constructor(private http: HttpClient) { }

  login(userCredentials: any): Observable<void> {  
    return this.http.post(this.baseURL + 'login', userCredentials).pipe(
      map( (resp: any) => {
        const user = resp;
        if(user) {
          localStorage.setItem('token', user.token);
          console.log(user.token);          
        }        
      })
    );
  }

  register(user: any) {
    return this.http.post(this.baseURL + 'register', user);
  }
}
