import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

       return next.handle(req).pipe(catchError( err => {

            if (!err.status) {
                return throwError('app server is down');
            }

            // const e = err as Response;
            const e = err;

            if (e.status === 401) {
                return throwError(e.statusText);
            }

            if ( e instanceof HttpErrorResponse) {
                const applicationError = e.headers.get('Application-Error');

                if (applicationError) {
                    return throwError(applicationError);
                }

                const servError = e.error;
                let validationErrors = '';

                if (servError.errors && typeof servError.errors === 'object' ) {

                    for (const key in servError.errors) {
                        if (servError.errors[key]) {
                            validationErrors += servError.errors[key] + '\n';
                        }
                    }
                }
                return throwError(validationErrors || servError || 'unknown error from http interceptor');
            }
        }
    ));
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass : ErrorInterceptor,
    multi: true
};
