import { HttpInterceptorFn } from '@angular/common/http';
import { BusyService } from '../services/busy.service';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { delay, finalize, identity } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  busyService.busy();

  return next(req).pipe(
    (environment.production? identity : delay(500)),
    finalize(() => busyService.idle())
  )
};
