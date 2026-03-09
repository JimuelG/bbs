import { Component, inject } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from "@angular/router";
import { AccountService } from '../../core/services/account.service';
import { MatMenuModule } from '@angular/material/menu';
import { AsyncPipe, CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    MatIconModule,
    MatMenuModule,
    CommonModule,
    RouterLink,
],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  accountService = inject(AccountService);
  private router = inject(Router);

  isMobileMenuOpen = false;
  baseUrl = "https://localhost:5001";

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      }
    })
  }
}
