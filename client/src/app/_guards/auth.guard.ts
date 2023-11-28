import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);



  return accountService.currentUser$.pipe(
    map(user => {
      if (user) {
        const requiredRoles = route.data['roles'] as string[];

        if(!requiredRoles || requiredRoles.some(role => user.role.includes(role))) {
        return true;
      } else {
        toastr.error('You dont have permission to access this page')
        return false;
      }
      }
      else {
        toastr.error('You are not allowed here');
        return false;
      }
    })
  );
};
