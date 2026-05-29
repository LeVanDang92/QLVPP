import { inject } from "@angular/core";
import { CanActivateFn, Router } from "@angular/router";
import { AppConstants } from "../common/constant";
import { TokenService } from "../services/token.service";

export const guestGuard : CanActivateFn = (route, state) => {

  const tokenService = inject(TokenService);
  const router = inject(Router);

  if (!tokenService.inValidToken()) {
      return true;
  }

  router.navigate(['/']); // Redirect to home or dashboard if already logged in
  return false;
}
