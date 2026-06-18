import { inject, Injectable } from '@angular/core';
import { AppConstants } from '../common/constant';
import { TokenResponse } from '../../features/Auth/models/tokenResponse';
import { throwError } from 'rxjs/internal/observable/throwError';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { jwtDecode } from 'jwt-decode';
import { JwtPayload } from '../../features/Auth/models/jwtPayload';
import { environment } from '../../../environments/environment.development';
import { catchError } from 'rxjs/internal/operators/catchError';
import { tap } from 'rxjs/internal/operators/tap';

@Injectable({
  providedIn: 'root',
})
export class TokenService {

    httpClient = inject(HttpClient);

   clearAuthData(): void {
    sessionStorage.removeItem(AppConstants.ACCESS_TOKEN_KEY);
    // localStorage.removeItem(AppConstants.REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(AppConstants.TOKEN_EXPIRY_KEY);
  }

   decodeToken(token: string): JwtPayload {
    return jwtDecode<JwtPayload>(token);
  }

  handleError(error: HttpErrorResponse) {
    console.error(error.message);

    return throwError(() => new Error(error.message || 'Server error occurred'));
    // Implement additional error handling logic as needed
  }

  // Save tokens to local storage
   saveToken(tokenResponse: TokenResponse) {
    sessionStorage.setItem(AppConstants.ACCESS_TOKEN_KEY, tokenResponse.accessToken);
    // localStorage.setItem(AppConstants.REFRESH_TOKEN_KEY, tokenResponse.refreshToken);
    sessionStorage.setItem(AppConstants.TOKEN_EXPIRY_KEY, tokenResponse.expiresAt);
  }

  // kiêm tra token có hợp lệ hay không (có tồn tại và chưa hết hạn)
  inValidToken(): boolean {
    const token = sessionStorage.getItem(AppConstants.ACCESS_TOKEN_KEY);
    if (!token) {
      return true;
    }

    try {
      const payload = this.decodeToken(token);
      const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
      return (payload.exp ?? 0) < currentTime; // Token is invalid if it has expired
    } catch (error) {
      console.error('Error decoding token:', error);
      return true; // Treat as invalid if decoding fails
    }
  }

refreshToken() {
  return this.httpClient.post<TokenResponse>(
    `${environment.apiUrl}/auth/refresh-token`,
    {},
    { withCredentials: true }
  ).pipe(
    tap((response) => {
      this.saveToken(response);
    }),
    catchError((error) => {
      console.error('Error refreshing token:', error);
      this.clearAuthData();
      return throwError(() => error);
    })
  );
}

}
