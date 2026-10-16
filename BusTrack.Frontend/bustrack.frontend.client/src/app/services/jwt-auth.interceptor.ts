import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, catchError, throwError } from 'rxjs';
import { SessionService } from './session.service';

@Injectable()
export class JwtAuthInterceptor implements HttpInterceptor {
  constructor(
    private readonly sessionService: SessionService,
    private readonly router: Router
  ) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    const token = this.sessionService.getAccessToken();
    const authorizedRequest = token
      ? request.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        })
      : request;

    return next.handle(authorizedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !request.url.includes('/AuthenticationControllerAPI/login')) {
          this.sessionService.clearSession();

          if (this.router.url !== '/enter-the-system') {
            void this.router.navigate(['/enter-the-system'], { replaceUrl: true });
          }
        }

        return throwError(() => error);
      })
    );
  }
}
