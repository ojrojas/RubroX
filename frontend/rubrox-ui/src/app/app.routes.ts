import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component')
      .then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'rubros',
    loadChildren: () => import('./features/rubros/rubros.routes')
      .then(m => m.rubrosRoutes),
    canActivate: [authGuard]
  },
  {
    path: 'movimientos',
    loadChildren: () => import('./features/movimientos/movimientos.routes')
      .then(m => m.movimientosRoutes),
    canActivate: [authGuard]
  },
  {
    path: 'flujos',
    loadChildren: () => import('./features/flujos/flujos.routes')
      .then(m => m.flujosRoutes),
    canActivate: [authGuard]
  },
  {
    path: 'callback',
    loadComponent: () => import('./core/auth/callback.component')
      .then(m => m.CallbackComponent)
  },
  {
    path: 'acceso-denegado',
    loadComponent: () => import('./core/auth/access-denied.component')
      .then(m => m.AccessDeniedComponent)
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
