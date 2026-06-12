import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { environment } from '../../../../environments/environment.development';
import { AccountService } from '../../../core/services/account.service';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-id-preview-modal',
  imports: [
    MatIcon,
    MatDialogModule
  ],
  templateUrl: './id-preview-modal.component.html',
  styleUrl: './id-preview-modal.component.scss',
})
export class IdPreviewModalComponent {
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);
  
  data = inject(MAT_DIALOG_DATA);
  baseApiUrl = environment.apiUrl;

  verifyResident(id: string) {
    this.accountService.verifyResident(id.toString()).subscribe({
      next: () => {
        this.snackbarService.success('Resident verified successfully');
      },
      error: () => {
        this.snackbarService.error('Failed to verify resident');
      }
    });
  }
}
