import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { TokenService } from "../services/token.service";
import { AuthenticationService } from "../../features/Auth/services/authentication.service";
import { catchError } from "rxjs/internal/operators/catchError";
import { map } from "rxjs/internal/operators/map";
import { of } from "rxjs";

export const authGuard : CanActivateFn = (route, state) => {

  const tokenService = inject(TokenService);
  const authService = inject(AuthenticationService);
  const router = inject(Router);

  // token tồn tại và hợp lệ, tiếp tục load thông tin người dùng khi nhấn F5
  if (!tokenService.inValidToken()) {

    return authService.loadMe().pipe(
    map(() => true),
    catchError(() => {
      tokenService.clearAuthData();
      return of(router.createUrlTree(['/auth/login']));
    }));
  }

  return router.createUrlTree(['/auth/login']); // Redirect to login if not authenticated
};

