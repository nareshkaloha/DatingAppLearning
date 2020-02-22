import { AuthService } from './../_services/auth.service';
import { catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { User } from '../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';


@Injectable()
export class MemberEditResolver implements Resolve<User>{

    constructor(private userService: UserService,
                private alerify: AlertifyService,
                private router: Router,
                private authService: AuthService) {
    }
    
    resolve(route: ActivatedRouteSnapshot): Observable<User> {

        const id = (this.authService.decodedToken).nameid;
        console.log(id);

        return this.userService.getUser(id).pipe(
            catchError(err => {
                this.alerify.error(err);
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
