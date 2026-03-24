import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map, of } from 'rxjs';

export const adminAuthGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.currentUser()?.role === 'Staff') {
    return of(true);
  }

  return accountService.getAuthState().pipe(
    map(auth => {
      const currentUser = accountService.currentUser();
      if (auth.isAuthenticated && currentUser?.role === 'Staff') {
        router.navigate(['admin/dashboard'])
        return true;
      } else {
        router.navigate(['admin/login', { queryParams: { returnUrl: state.url}}]);
        return false;
      }
    })
  )
  
};
