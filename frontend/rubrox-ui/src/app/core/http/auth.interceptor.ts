import { inject } from '@angular/core';
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';

/** Interceptor que adjunta el Bearer token en cada petición a la API. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const oauthService = inject(OAuthService);
  const token = oauthService.getAccessToken();

  if (token && req.url.includes('/api/')) {
    const authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(authReq).pipe(
      catchError(handleError)
    );
  }

  return next(req).pipe(catchError(handleError));
};

function handleError(error: HttpErrorResponse) {
  let message = 'Error desconocido';

  if (error.status === 401) message = 'Sesión expirada. Por favor inicie sesión nuevamente.';
  else if (error.status === 403) message = 'No tiene permisos para realizar esta acción.';
  else if (error.status === 404) message = 'El recurso solicitado no fue encontrado.';
  else if (error.status === 409) message = error.error?.error ?? 'Conflicto: ' + error.message;
  else if (error.status === 422) message = error.error?.detail ?? 'Error de validación.';
  else if (error.status >= 500) message = 'Error del servidor. Intente más tarde.';
  else if (error.error?.error) message = error.error.error;

  return throwError(() => new Error(message));
}
