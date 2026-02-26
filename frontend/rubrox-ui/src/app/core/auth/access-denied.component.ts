import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'rx-access-denied',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="text-align:center;padding:4rem">
      <h1>Acceso denegado</h1>
      <p>No tiene permisos para acceder a este recurso.</p>
      <a routerLink="/dashboard">Volver al inicio</a>
    </div>
  `
})
export class AccessDeniedComponent {}
