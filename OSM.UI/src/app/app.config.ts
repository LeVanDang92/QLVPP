import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth-interceptor';
import { errorInterceptor } from './core/interceptors/errorInterceptor';

import { provideTranslateService } from '@ngx-translate/core';

import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { AppConstants } from './core/common/constant';

const httpInterceptors = [authInterceptor, errorInterceptor];

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors(httpInterceptors)),

    provideTranslateService({
      fallbackLang: AppConstants.APP_DEFAULT_LANGUAGE,
      loader: provideTranslateHttpLoader({
        prefix: './assets/i18n/',
        suffix: '.json',
      }),
    }),
  ],
};
