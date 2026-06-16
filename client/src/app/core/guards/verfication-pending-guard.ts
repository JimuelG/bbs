import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

export const verficationPendingGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  const user = accountService.currentUser();
  
  if (!user) {
    return router.createUrlTree(['/account/login']);
  }

  const isAdmin =
    user.role === 'Admin' ||
    user.role?.includes('Admin');

  const verified = user.isIdVerified === true;

  if (isAdmin || verified) {
    return router.createUrlTree(['/home']);
  }

  return true;
};
