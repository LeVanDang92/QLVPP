import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { Login } from '../models/login';
import { TokenResponse } from '../models/tokenResponse';
import { catchError, Observable, of, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { TokenService } from '../../../core/services/token.service';
import { CurrentUserResponse, MenuSection } from '../models/current-user-response.model';
import { AppConstants } from '../../../core/common/constant';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  baseUrl = `${environment.apiUrl}/auth`;
  HttpClient = inject(HttpClient);
  router = inject(Router);
  tokenService = inject(TokenService);


  currentUser = signal<CurrentUserResponse | null>(null);
  menus = signal<MenuSection[]>([]);
  permissions = signal<string[]>([]);

  private fullNameKey = 'full_name';
  fullName = signal<string | null>(localStorage.getItem(this.fullNameKey));

  login(request: Login): Observable<TokenResponse> {
    return this.HttpClient.post<TokenResponse>(`${this.baseUrl}/login`, request, {
      withCredentials: true,
    }).pipe(
      tap((response) => {
        this.tokenService.saveToken(response);

        const payload = this.tokenService.decodeToken(response.accessToken);
        localStorage.setItem(this.fullNameKey, payload.fullName || '');
        this.fullName.set(payload.fullName || '');
      }),
      catchError((error) => {
        this.tokenService.handleError(error);
        return throwError(() => new Error(error.message || 'Server error occurred'));
      }),
    );
  }

  logout(): void {
    // Angular HTTP observables complete after emitting once, but take(1) clarifies intent.
    this.HttpClient.post(`${this.baseUrl}/logout`, {}, { withCredentials: true })
      .pipe
      // take(1) ensures the observable completes after the first emission
      // and documents the expected behavior.
      // If you remove take(1), the observable still completes after one emission.
      // import { take } from 'rxjs/operators'; at the top if not already imported.
      // take(1)
      ()
      .subscribe({
        next: () => {
          console.log('Logged out successfully');
          this.tokenService.clearAuthData();
          this.clearData();
          this.router.navigate(['/auth/login']);
        },
        error: (error) => {
          console.error('Logout failed', error.message);
          alert('Logout failed');
        },
      });
  }

private clearData(): void {
    localStorage.removeItem(this.fullNameKey);
    this.fullName.set(null);
    this.currentUser.set(null);
    this.menus.set([]);
    this.permissions.set([]);
  }

  // Get current user info after login or when app initializes : role, permissions, menu
loadMe(): Observable<CurrentUserResponse> {

   if (this.currentUser()) {
      // If user data is already loaded, return it as an observable
      return of(this.currentUser()!);
    }

    const token = localStorage.getItem(AppConstants.ACCESS_TOKEN_KEY);

    if (token) {
      const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}` // Đính kèm token vào đây
    });

    return this.HttpClient.get<CurrentUserResponse>(`${this.baseUrl}/me`, { headers }).pipe(
      tap((response: CurrentUserResponse) => {
        this.currentUser.set(response);
        this.menus.set(response.menus);
        this.permissions.set(response.permissions);

        console.log('Current user loaded:', response);
      }),
      catchError((error) => {
        this.tokenService.handleError(error);
        return throwError(() => new Error(error.message || 'Server error occurred'));
      }),
    );
    }
    else {
      // Nếu không có token, trả về observable lỗi hoặc giá trị mặc định
      return throwError(() => new Error('No access token found'));
    }
  }

hasPermission(permission: string): boolean {
    return this.permissions().includes(permission);
  }
}
