import { Routes } from '@angular/router';
import { roleGuard } from '../../core/auth/auth.guard';

export const flujosRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./bandeja/bandeja-aprobacion.component').then(m => m.BandejaAprobacionComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./detalle/flujo-detalle.component').then(m => m.FlujoDetalleComponent)
  }
];
