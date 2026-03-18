import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ContentTypeInterceptor } from './interceptors/content-type.interceptor';
import { AuthorizationInterceptor } from './interceptors/authorization-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([ContentTypeInterceptor, AuthorizationInterceptor])),
  ],
};
