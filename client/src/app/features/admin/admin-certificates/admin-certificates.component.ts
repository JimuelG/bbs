import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CreateCertificateComponent } from '../../../shared/components/create-certificate/create-certificate.component';
import { CertificatesService } from '../../../core/services/certificates.service';
import { Certificate } from '../../../shared/models/certificate';

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
  private certificateService = inject(CertificatesService);

  certificates: Certificate[] = [];
  
  loadCertificates() {
    this.certificateService.loadCertificates().subscribe({
      next: (result) => {
        this.certificates = result;
      }
    });
  }
  openCertificateModal(): void {
    const dialogRef = this.dialog.open(CreateCertificateComponent, {
      width: 'auto',
      maxWidth: '95vw',
      panelClass: 'custom-dialog-container',
      disableClose: true
    });

      dialogRef.afterClosed().subscribe(result => {
      if (result && result.pdfUrl) {
        const apiUrl = 'http:/localhost:5001'
        window.open(`${apiUrl}${result.pdfUrl}`, '_blank');
        this.loadCertificates();
      }
    })
  }
}
