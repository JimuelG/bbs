import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { map } from 'rxjs';

export const verificationGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const user = accountService.currentUser();

  console.log('Current user in guard:', user);

  if (user) {
    const verified = user.isIdVerified;
    const isAdmin = user.role?.includes('Admin') || user.role === 'Admin';
    if (isAdmin || verified) {
      return true;
    } 
  }

  router.navigate(['/account/verification-pending']);
  return false;
};
