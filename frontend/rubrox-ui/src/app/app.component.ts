import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { authConfig } from './core/auth/auth-config';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet />`
})
export class AppComponent implements OnInit {
  private readonly oauth = inject(OAuthService);

  ngOnInit(): void {
    this.oauth.configure(authConfig);
    this.oauth.loadDiscoveryDocumentAndTryLogin();
  }
}
