import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AppConstants } from '../common/constant';
import { throwError } from 'rxjs/internal/observable/throwError';
import { catchError } from 'rxjs/internal/operators/catchError';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/internal/operators/finalize';
import { switchMap } from 'rxjs/internal/operators/switchMap';

let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const tokenService = inject(TokenService);
  const router = inject(Router);
  const token = sessionStorage.getItem(AppConstants.ACCESS_TOKEN_KEY);

  console.log('Intercepting request to:', req.url);

  // url này không cần thêm header Authorization
   const isAuthUrl =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/refresh-token') ||
    req.url.includes('/auth/logout');


  // If a token exists, clone the request and add the Authorization header
  if (token && !isAuthUrl) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      },
      withCredentials: true,
    });
  } else {
    req = req.clone({
      withCredentials: true,
    });
  }

  return next(req).pipe(

    catchError((error : HttpErrorResponse) => {
      if (error.status === 401 && !isAuthUrl) {
        // Handle unauthorized error, e.g., redirect to login page
        console.error('Unauthorized access - redirecting to login');
        // You can use Angular's Router to navigate to the login page
        // router.navigate(['/auth/login']);

        if(!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(null);

          return tokenService.refreshToken().pipe(
            catchError((refreshError) => {
              tokenService.clearAuthData();
              router.navigate(['/auth/login']);
              return throwError(() => new Error(refreshError.message || 'Server error occurred'));
            }),
            switchMap((newTokenResponse) => {
              tokenService.saveToken(newTokenResponse);
              refreshTokenSubject.next(newTokenResponse.accessToken);
              return next(req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newTokenResponse.accessToken}`
                },
                withCredentials: true,
              }));
            }),
            finalize(() => {
              isRefreshing = false;
            })
          );

        }

        return refreshTokenSubject.pipe(
          switchMap((newToken) => {
            if (newToken) {
              return next(req.clone({
                setHeaders: {
                  Authorization: `Bearer ${newToken}`
                },
                withCredentials: true,
              }));
            } else {
              router.navigate(['/auth/login']);
              return throwError(() => new Error('No new token available'));
            }
          })
        );
      }
      return throwError(() => new Error(error.message || 'Server error occurred'));
    })

  );
};
