import { Routes } from '@angular/router';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout.component';
import { authRoutes } from './features/Auth/auth.routes';
import { authGuard } from './core/guard/authGuard';
import { guestGuard } from './core/guard/guestGuard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'role',
        loadComponent: () =>
          import('./features/Base_setup/pages/role/role').then((m) => m.Role),
           data : { breadcrumb: 'Settings > Role Setup' , title: 'Role Setup' , sectionTitle: 'Role Detail Information' }
      },
      {
        path: 'menu',
        loadComponent: () =>
          import('./features/Base_setup/pages/menu/menu').then((m) => m.Menu),
           data : { breadcrumb: 'Settings > Menu Setup' , title: 'Menu Setup' , sectionTitle: 'Menu Detail Information' }
      },
       {
        path: 'menurole',
        loadComponent: () =>
          import('./features/Base_setup/pages/menurole/menurole').then((m) => m.Menurole),
           data : { breadcrumb: 'Settings > Menu Role Setup' , title: 'Menu Role Setup' , sectionTitle: 'Menu Role Detail Information' }
      },
      {
        path :'register',
        loadComponent: () =>
          import('./features/Base_setup/pages/register/register').then((m) => m.Register),
           data : { breadcrumb: 'Settings > User Setup' , title: 'User Setup' , sectionTitle: 'User Detail Information' }
      },
      {
        path: 'workspace/:slug',
        loadComponent: () =>
          import('./features/workspace-page/workspace-page.component').then((m) => m.WorkspacePageComponent),
      },
    ],
  },
  {
    path: 'auth',
    loadChildren: () => import('./features/Auth/auth.routes').then((m) => m.authRoutes),
  },
  {
    path: '**',
    loadComponent: () => import('./features/errors/not-found/not-found').then((m) => m.NotFound),
  }
];
