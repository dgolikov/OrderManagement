import { Routes } from '@angular/router';
import { ProductsComponent } from './layout/products/products';
import { SignInComponent } from './layout/sign-in/sign-in';
import { OrdersComponent } from './layout/orders/orders';
import { AuthGuard } from './guards/auth.guard';
import { NotAuthGuard } from './guards/not-auth.guard';

export const routes: Routes = [
  {
    path: 'sign-in',
    component: SignInComponent,
    canActivate: [NotAuthGuard],
  },
  {
    path: '',
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'products', pathMatch: 'full' },
      { path: 'products', component: ProductsComponent },
      { path: 'orders', component: OrdersComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
