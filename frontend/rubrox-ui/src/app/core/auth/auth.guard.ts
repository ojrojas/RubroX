import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

/** Guard que verifica si el usuario tiene sesión activa. */
export const authGuard: CanActivateFn = (route, state) => {
  const oauthService = inject(OAuthService);
  const router = inject(Router);

  if (oauthService.hasValidAccessToken()) {
    return true;
  }

  oauthService.initCodeFlow(state.url);
  return false;
};

/** Guard que verifica si el usuario tiene el rol requerido. */
export const roleGuard: CanActivateFn = (route, state) => {
  const oauthService = inject(OAuthService);
  const router = inject(Router);
  const requiredRoles: string[] = route.data['roles'] ?? [];

  if (!oauthService.hasValidAccessToken()) {
    oauthService.initCodeFlow(state.url);
    return false;
  }

  const claims = oauthService.getIdentityClaims() as Record<string, unknown>;
  const userRoles: string[] = (claims?.['roles'] as string[]) ?? [];

  const hasRole = requiredRoles.some(r => userRoles.includes(r));
  if (!hasRole) {
    router.navigate(['/acceso-denegado']);
    return false;
  }

  return true;
};
