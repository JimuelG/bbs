import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CreateCertificateComponent } from '../../../shared/components/create-certificate/create-certificate.component';
import { CertificatesService } from '../../../core/services/certificates.service';
import { Certificate } from '../../../shared/models/certificate';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'app-admin-certificates',
  imports: [
    MatIcon,
  ],
  templateUrl: './admin-certificates.component.html',
  styleUrl: './admin-certificates.component.scss',
})
export class AdminCertificatesComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private certificateService = inject(CertificatesService);

  certificates: Certificate[] = [];
  apiUrl = 'https://localhost:5001'

  ngOnInit(): void {
    this.loadCertificates();
  }
  
  loadCertificates() {
    this.certificateService.loadCertificates().subscribe({
      next: (result) => {
        this.certificates = result;
      }
    });
  }

  viewCert(ref: string) {
    window.open(`${this.apiUrl}/certificates/${ref}.pdf`, '_blank');

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'A',
        message: 'Did you print the certificate?'
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
        window.open(`${this.apiUrl}${result.pdfUrl}`, '_blank');
        this.loadCertificates();
      } 
    })
  }

  editCert(cert: Certificate) {
    const dialogRef = this.dialog.open(CreateCertificateComponent, {
      width: '500px',
      data: cert || null,
    })

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadCertificates();
    })
  }
}
