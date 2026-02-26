import { Routes } from '@angular/router';
import { roleGuard } from '../../core/auth/auth.guard';

export const movimientosRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./list/movimientos-list.component').then(m => m.MovimientosListComponent)
  },
  {
    path: 'cdp/nuevo',
    loadComponent: () => import('./cdp/cdp-form.component').then(m => m.CdpFormComponent),
    canActivate: [roleGuard],
    data: { roles: ['admin_presupuestal', 'analista'] }
  }
];
