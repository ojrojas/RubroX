import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
@Component({ selector: 'rx-callback', standalone: true, template: '<p>Autenticando...</p>' })
export class CallbackComponent implements OnInit {
  private readonly oauth = inject(OAuthService);
  private readonly router = inject(Router);
  async ngOnInit() {
    await this.oauth.loadDiscoveryDocumentAndTryLogin();
    if (this.oauth.hasValidAccessToken()) {
      const redirectUrl = sessionStorage.getItem('redirectUrl') ?? '/dashboard';
      sessionStorage.removeItem('redirectUrl');
      this.router.navigateByUrl(redirectUrl);
    }
  }
}
