import { Component, inject } from '@angular/core';
import { CertificateInfo } from '../../shared/models/certificateInfo';
import { Router } from '@angular/router';

@Component({
  selector: 'app-certificates',
  imports: [],
  templateUrl: './certificates.component.html',
  styleUrl: './certificates.component.scss',
})
export class CertificatesComponent {
  private router = inject(Router);

  certificates: CertificateInfo[] = [
      { 
        type: 1, name: 'Barangay Clearance', isOpen: false,
        description: 'Used for employment, business permits, or legal transactions.',
        requirements: ['Valid ID'] 
      },
      { 
        type: 2, name: 'Certificate of Residency', isOpen: false,
        description: 'Proof that you are a bonafide resident of this Barangay.',
        requirements: ['Valid ID', 'Proof of Address'] 
      },
      { 
        type: 3, name: 'Certificate of Indigency', isOpen: false,
        description: 'Requested for financial assistance, medical aid, or scholarships.',
        requirements: ['Case Study from DSWD or Affidavit of Low Income'] 
      },
      { 
        type: 4, name: 'First Time Job Seeker', isOpen: false,
        description: 'RA 11261: Waives fees for government-issued documents for first-time jobs.',
        requirements: ['Oath of Undertaking', 'Minor (if applicable) must be accompanied'] 
      },
      // Add the rest of your enums here...
      { 
        type: 17, name: 'Tree Cutting Permit', isOpen: false,
        description: 'Necessary clearance before cutting trees within private or public lands.',
        requirements: ['DENR Clearance', 'Photos of the tree'] 
      }
    ];

  toggleAccordion(index: number) {
    this.certificates[index].isOpen = !this.certificates[index].isOpen;
  }

  onRequest(cert: CertificateInfo) {
    switch (cert.type) {
      case 1:
        this.router.navigate(['/request-certificates/', cert.type]);
        break;
      default:
        this.router.navigate(['/request-certificates']);
        break;
    }
  }
}
