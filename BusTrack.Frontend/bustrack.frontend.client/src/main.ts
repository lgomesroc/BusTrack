
import { registerLocaleData } from '@angular/common';
import localePtBr from '@angular/common/locales/pt';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

registerLocaleData(localePtBr);

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch(err => console.error(err));
