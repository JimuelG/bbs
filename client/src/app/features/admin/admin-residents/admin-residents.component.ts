import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { AccountService } from '../../../core/services/account.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Resident } from '../../../shared/models/residents';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-admin-residents',
  imports: [
    MatIcon,
    CurrencyPipe
  ],
  templateUrl: './admin-residents.component.html',
  styleUrl: './admin-residents.component.scss',
})
export class AdminResidentsComponent implements OnInit {
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);

  residents: Resident[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadResidents();
  }

  loadResidents() {
    this.loading = true;
    this.accountService.getAllResidents().subscribe({
      next: (data) => {
        this.residents = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  verifyResident(id: string) {
    this.accountService.verifyResident(id.toString()).subscribe({
      next: () => {
        this.snackbarService.success('Resident verified successfully');
        this.loadResidents();
      },
      error: () => {
        this.snackbarService.error('Failed to verify resident');
      }
    });
  }

  viewId(url: string) {
    if (url) window.open(url, '_blank');
  }
}
