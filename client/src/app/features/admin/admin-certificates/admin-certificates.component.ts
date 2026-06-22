import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { CreateCertificateComponent } from '../../../shared/components/create-certificate/create-certificate.component';
import { CertificatesService } from '../../../core/services/certificates.service';
import { Certificate } from '../../../shared/models/certificate';
import { CertificateTypePipe } from '../../../shared/pipes/certificate-type-pipe';
import { environment } from '../../../../environments/environment.development';
import { Pagination } from '../../../shared/models/pagination';
import { CertificateParams } from '../../../shared/models/certificateParams';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PdfPreviewComponent } from '../../../shared/components/pdf-preview/pdf-preview.component';

@Component({
  selector: 'app-admin-certificates',
  imports: [
    MatIcon,
    CertificateTypePipe,
    CommonModule,
    FormsModule
  ],
  templateUrl: './admin-certificates.component.html',
  styleUrl: './admin-certificates.component.scss',
})
export class AdminCertificatesComponent implements OnInit {
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);
  private certificateService = inject(CertificatesService);
  private sanitizer = inject(DomSanitizer);

  certificates: Certificate[] = [];
  certificatePagination?: Pagination<Certificate>;
  certificateParams = new CertificateParams();
  totalCount = 0;
  pageSizeOptions = [10,20,30];
  
  private apiOrigin = environment.apiUrl.replace(/\/api\/$/, '');

  private getPublicFileUrl(path: string): string {
    if (!path) return '';
    
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    let normalizedPath = path.startsWith('/') ? path : `/${path}`;

    normalizedPath = normalizedPath.replace(/^\/api\/api\//, '/api/');

    if (!normalizedPath.startsWith('/api/')) {
      normalizedPath = `/api${normalizedPath}`;
    }

    return `${this.apiOrigin}${normalizedPath}` .replace('/api/api/', '/api/');
  }

  editingStatusId: number | null = null;

  statuses = [
    'Pending',
    'Approved',
    'Printed',
    'Released',
    'Denied'
  ];

  private previewObjectUrl: string | null = null;

  ngOnInit(): void {
    this.loadCertificates();
  }
  
  loadCertificates() {
    this.certificateService.getAllCertificates(this.certificateParams).subscribe({
      next: (response) => {
        this.certificatePagination = response;
        this.certificates = response.data;
        this.totalCount = response.count;
      }
    })
  }

  onPageChange(event: any) {
    this.certificateParams.pageIndex = event.pageIndex + 1;
    this.certificateParams.pageSize = event.pageSize;
    this.loadCertificates();
  }

  updatePage(newPageIndex: number) {
    this.certificateParams.pageIndex = newPageIndex;
    this.loadCertificates();
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.certificateParams.pageSize = parseInt(select.value);
    this.loadCertificates();
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }

  onSearchChange() {
    this.certificateParams.pageIndex = 1;
    this.loadCertificates();
  }

  onSortChange() {
    this.certificateParams.pageIndex = 1;
    this.loadCertificates();
  }

  onStatusChange() {
    this.certificateParams.pageIndex = 1;
    this.loadCertificates();
  }

  viewCert(ref: string) {

    this.certificateService.generateCertificatePdf(ref).subscribe({
      next: () => {
        const url = this.getPublicFileUrl(`/api/certificates/${ref}.pdf`);
        window.open(url, '_blank');
      }
    });

  }

  printCert(cert: Certificate) {
    this.certificateService.printCertificate(cert.referenceNumber).subscribe({
      next: (response) => {  
        cert.status = response.status;
        cert.issuedAt = response.issuedAt;

        window.open(this.getPublicFileUrl(response.pdfUrl), '_blank');
        
        this.loadCertificates();
      },
      error: () => {
        this.snackbarService.error('Failed to print certificate');
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
        window.open(this.getPublicFileUrl(result.pdfUrl), '_blank');
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

  startEditStatus(cert: Certificate) {
    this.editingStatusId = cert.id;
  }

  changeStatus(cert: Certificate, status: string) {
    if (cert.status === status) {
      this.editingStatusId = null;
    }

    this.certificateService.updateCertificateStatus(cert.id, status).subscribe({
      next: (response) => {
        cert.status = response.status;

        if (response.issuedAt) {
          cert.issuedAt = response.issuedAt;
        }

        this.editingStatusId = null;
        this.snackbarService.success('Status updated successfully');
      },
      error: () => {
        this.snackbarService.error('Failed to update status');
      }
    })
  }

  previewCert(cert: Certificate) {
    this.certificateService.previewCertficatePdf(cert.referenceNumber).subscribe({
      next: (blob) => {
        if (this.previewObjectUrl) {
          URL.revokeObjectURL(this.previewObjectUrl);
        }

        this.previewObjectUrl = URL.createObjectURL(blob);

        const safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.previewObjectUrl);

        const dialogRef = this.dialog.open(PdfPreviewComponent, {
          width: '95vw',
          maxWidth: '1100px',
          height: '90vh',
          panelClass: 'pdf-preview-dialog',
          data: {
            title: `${cert.certificateType} - ${cert.fullName}`,
            pdfUrl: safeUrl
          }
        });

        dialogRef.afterClosed().subscribe(() => {
          if (this.previewObjectUrl) {
            URL.revokeObjectURL(this.previewObjectUrl);
            this.previewObjectUrl = null;
          }
        });
      },
      error: () => {
        this.snackbarService.error('Failed to preview PDF');
      }
    });
  }

  getAvailableStatuses(cert: Certificate): string[] {
    switch (cert.status) {
      case 'Pending':
          return ['Pending', 'Approved', 'Denied'];
      
      case 'Approved':
          return ['Approved', 'Printed', 'Denied'];
        
      case 'Printed':
        return ['Printed', 'Released'];

      case 'Released':
          return ['Released'];

      case 'Denied':
          return ['Denied'];

      default:
          return ['Pending', 'Approved', 'Denied'];
    }
  }

}