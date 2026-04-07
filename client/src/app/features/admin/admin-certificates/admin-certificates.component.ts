import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CreateCertificateComponent } from '../../../shared/components/create-certificate/create-certificate.component';
import { CertificatesService } from '../../../core/services/certificates.service';
import { Certificate } from '../../../shared/models/certificate';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { CertificateTypePipe } from '../../../shared/pipes/certificate-type-pipe';

@Component({
  selector: 'app-admin-certificates',
  imports: [
    MatIcon,
    CertificateTypePipe
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

    this.certificateService.generateCertificatePdf(ref).subscribe({
      next: (pdfBlob) => {
        const url = this.apiUrl + `/certificates/${ref}.pdf`;
        window.open(url, '_blank');
        // Clean up the URL object after opening
        setTimeout(() => {
          window.URL.revokeObjectURL(url);
        }, 100);
      }
    });

  }

  printCert(cert: Certificate) {
    this.certificateService.generateCertificatePdf(cert.referenceNumber).subscribe({
      next: (pdfBlob) => {  
        const url = this.apiUrl + `/certificates/${cert.referenceNumber}.pdf`;
        const printWindow = window.open(url, '_blank');
        printWindow?.addEventListener('load', () => {
          printWindow.focus();
          printWindow.print();
        }); 
        // Clean up the URL object after opening
        setTimeout(() => {
          window.URL.revokeObjectURL(url);
        }, 100);
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