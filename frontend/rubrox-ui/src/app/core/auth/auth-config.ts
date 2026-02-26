import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from '../../../environments/environment';

export const authConfig: AuthConfig = {
  issuer: environment.identityUrl,
  redirectUri: window.location.origin + '/callback',
  postLogoutRedirectUri: window.location.origin,
  silentRefreshRedirectUri: window.location.origin + '/silent-renew.html',
  clientId: 'rubrox-spa',
  responseType: 'code',
  scope: 'openid profile email offline_access rubrox.rubros rubrox.movimientos rubrox.flujos rubrox.reportes',
  useSilentRefresh: true,
  silentRefreshTimeout: 5000,
  timeoutFactor: 0.75,
  sessionChecksEnabled: false,
  showDebugInformation: !environment.production,
  clearHashAfterLogin: true,
  // PKCE está habilitado por defecto en angular-oauth2-oidc cuando responseType='code'
};
