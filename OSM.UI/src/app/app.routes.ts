import { Routes } from '@angular/router';
import { MainLayoutComponent } from './core/layout/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
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
        path: 'operation',
        loadComponent: () =>
          import('./features/operation/operation.component').then((m) => m.OperationComponent),
      },
      {
        path: 'product-mix-operation-capa',
        loadComponent: () =>
          import('./features/product-mix-operation-capa/product-mix-operation-capa.component').then(
            (m) => m.ProductMixOperationCapaComponent,
          ),
      },
      {
        path: 'workspace/:slug',
        loadComponent: () =>
          import('./features/workspace-page/workspace-page.component').then((m) => m.WorkspacePageComponent),
      },
    ],
  },
];
