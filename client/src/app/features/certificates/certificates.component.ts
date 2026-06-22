import { Component, inject, OnInit } from '@angular/core';
import { CertificateInfo } from '../../shared/models/certificateInfo';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { SnackbarService } from '../../core/services/snackbar.service';

@Component({
  selector: 'app-certificates',
  imports: [],
  templateUrl: './certificates.component.html',
  styleUrl: './certificates.component.scss',
})
export class CertificatesComponent implements OnInit{
  private router = inject(Router);
  private http = inject(HttpClient);
  private snackbarService = inject(SnackbarService);

  certificates: CertificateInfo[] = [];

  ngOnInit(): void {
    this.loadCertifications();
  }

  loadCertifications() {
    this.http.get<CertificateInfo[]>('/public/SeedData/certificates.json').subscribe({
      next: (data) => {
        this.certificates = data.map(cert => ({
          ...cert,
          isOpen: false
        }));
      },
      error: (err) => {
        this.snackbarService.error(`Failed to load certificates JSON: ${err}`);
      }
    })
  }

  toggleAccordion(index: number) {
    this.certificates[index].isOpen = !this.certificates[index].isOpen;
  }

  onRequest(cert: CertificateInfo) {
    this.router.navigate(['/request-certificates', cert.type])
  }
}
