import { AuthService } from '../_services/auth.service';
import { catchError } from 'rxjs/operators';
import { of, Observable } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Message } from '../_models/message';


@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
    private pageNumber = 1;
    private pageSize = 10;
    private messageContainer = 'Unread';

    constructor(private userService: UserService,
                private alertify: AlertifyService,
                private router: Router,
                private authService: AuthService) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {

        const id = (this.authService.decodedToken).nameid;
        console.log(id);

        return this.userService.getMessages(id, this.pageNumber, this.pageSize, this.messageContainer).pipe(
            catchError(err => {
                this.alertify.error(err);
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}
