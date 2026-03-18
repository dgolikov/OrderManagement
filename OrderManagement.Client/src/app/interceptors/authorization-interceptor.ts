import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';

export const AuthorizationInterceptor: HttpInterceptorFn = (req, next) => {
  const authService: AuthService = inject(AuthService);
  let headers = authService.addAuthorizationHeader(req.headers);
  return next(req.clone({ headers: headers }));
};
