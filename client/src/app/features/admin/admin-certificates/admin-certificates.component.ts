import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CreateCertificateComponent } from '../../../shared/components/create-certificate/create-certificate.component';

@Component({
  selector: 'app-admin-certificates',
  imports: [
    MatIcon,
  ],
  templateUrl: './admin-certificates.component.html',
  styleUrl: './admin-certificates.component.scss',
})
export class AdminCertificatesComponent {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  
  openCertificateModal(): void {
    const dialogRef = this.dialog.open(CreateCertificateComponent, {
      width: 'auto',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        
      }
    })
  }
}
