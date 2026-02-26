import { Routes } from '@angular/router';
import { roleGuard } from '../../core/auth/auth.guard';

export const rubrosRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./list/rubros-list.component').then(m => m.RubrosListComponent)
  },
  {
    path: 'nuevo',
    loadComponent: () => import('./form/rubro-form.component').then(m => m.RubroFormComponent),
    canActivate: [roleGuard],
    data: { roles: ['admin_presupuestal', 'analista'] }
  },
  {
    path: ':id',
    loadComponent: () => import('./detail/rubro-detail.component').then(m => m.RubroDetailComponent)
  }
];
