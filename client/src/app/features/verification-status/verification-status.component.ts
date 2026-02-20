import { Component, inject } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-verification-status',
  imports: [
    MatIcon
  ],
  templateUrl: './verification-status.component.html',
  styleUrl: './verification-status.component.scss',
})
export class VerificationStatusComponent {
  private accountService = inject(AccountService);

  logout() {
    this.accountService.logout();
  }
}
