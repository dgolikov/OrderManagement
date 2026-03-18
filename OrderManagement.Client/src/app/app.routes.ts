import { Routes } from '@angular/router';
import { ProductsComponent } from './layout/products/products';
import { SignInComponent } from './layout/sign-in/sign-in';
import { AuthGuard } from './guards/auth.guard';
import { NotAuthGuard } from './guards/not-auth.guard';

export const routes: Routes = [
  {
    component: ProductsComponent,
    path: '',
    pathMatch: 'full',
    canActivate: [AuthGuard],
  },
  {
    component: SignInComponent,
    path: 'sign-in',
    pathMatch: 'full',
    canActivate: [NotAuthGuard],
  },
  {
    component: ProductsComponent,
    path: '**',
    redirectTo: '',
  },
];
