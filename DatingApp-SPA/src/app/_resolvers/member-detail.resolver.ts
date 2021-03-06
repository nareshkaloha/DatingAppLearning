import { catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { User } from './../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';


@Injectable()
export class MemberDetailResolver implements Resolve<User>{

    constructor(private userService: UserService,
                private alerify: AlertifyService,
                private router: Router) {
    }
    
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(route.params['id']).pipe(
            catchError(err => {
                this.alerify.error(err);
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
