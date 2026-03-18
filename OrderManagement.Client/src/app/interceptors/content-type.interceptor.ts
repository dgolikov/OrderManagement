import { HttpInterceptorFn } from '@angular/common/http';

export const ContentTypeInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req.clone({ headers: req.headers.set('Content-Type', 'application/json') }));
};
