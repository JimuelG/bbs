import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { AccountService } from '../../../core/services/account.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Resident } from '../../../shared/models/residents';
import { CurrencyPipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { IdPreviewModalComponent } from '../../../shared/components/id-preview-modal/id-preview-modal.component';

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
  baseUrl = "https://localhost:5001";
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);
  private dialog = inject(MatDialog);

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

  viewId(resident: Resident) {
    const dialogRef = this.dialog.open(IdPreviewModalComponent, {
      data: {
        idUrl: `${resident.idUrl}`,
        name: `${resident.firstName} ${resident.lastName}`,
        isVerified: resident.isIdVerified
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'verified') {
        this.verifyResident(resident.id);
      }
    });
  }
}
