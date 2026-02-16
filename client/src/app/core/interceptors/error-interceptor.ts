import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { SnackbarService } from '../services/snackbar.service';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbar = inject(SnackbarService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 400) {
        if (err.error.errors) {
          const modelstateErrors = [];
          for (const key in err.error.errors) {
            if (err.error.errors[key]) {
              modelstateErrors.push(err.error.errors[key])
            }
          }
          throw modelstateErrors.flat();
        } else {
          snackbar.error(err.error.title || err.error)
        }
      }
      if (err.status === 401) {
        snackbar.error(err.error.title || err.error)
      }
      if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }
      if (err.status === 500) {
        const navigationExtras: NavigationExtras = {state: {error: err.error}}
        router.navigateByUrl('/server-error', navigationExtras);
      }
      return throwError(() => err)
    })
  )
};
