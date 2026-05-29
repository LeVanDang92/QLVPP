import { Routes } from '@angular/router';
import { guestGuard } from '../../core/guard/guestGuard';

export const authRoutes : Routes = [
  {
    path: 'login',
    // canActivate: [guestGuard],
    loadComponent: () => import('./login/login').then(m => m.Login)
  }
];
