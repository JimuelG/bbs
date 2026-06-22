import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CertificatesService } from '../../../core/services/certificates.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CertificateInfo } from '../../../shared/models/certificateInfo';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-certificate-request',
  imports: [
    CommonModule,
    MatIcon,
    ReactiveFormsModule
  ],
  templateUrl: './certificate-request.component.html',
  styleUrl: './certificate-request.component.scss',
})
export class CertificateRequestComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private certificateService = inject(CertificatesService);
  private snackbarService = inject(SnackbarService);
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  certificateForm!: FormGroup;
  certificateInfo?: CertificateInfo;

  isSubmitting = false;

  fieldLabels: Record<string, string> = {
    fullName: 'Full Name',
    civilStatus: 'Civil Status',
    age: "Age",
    birthDate: 'Birth Date',
    purok: 'Purok',
    purpose: 'Purpose',
    stayDuration: 'Years of Stay',
    gender: 'Gender',
    placeOfBirth: 'Place of Birth',
    address: 'Complete Address',
    fee: 'Fee'
  };

  fieldPlaceholders: Record<string, string> = {
    fullName: 'e.g. DELA CRUZ, JUAN P.',
    age: 'e.g. 25',
    placeOfBirth: 'e.g Tarlac City',
    purpose: 'e.g. Employment, Scholarship, Local ID',
    stayDuration: 'e.g. Since birth / 10 years',
    address: 'House No. / Street / Barangay',
  };

  ngOnInit(): void {
    this.loadCertificateTemplate();
  }

  private loadCertificateTemplate(): void {
    const typeId = Number(this.route.snapshot.paramMap.get('id') ?? 0);

    this.http.get<CertificateInfo[]>('/public/SeedData/certificates.json').subscribe({
      next: (certificates) => {
        const selected = certificates.find(x => x.type === typeId);

        if (!selected) {
          this.snackbarService.error('Certificate template not found.');
          this.router.navigate(['/certifacates']);
          return;
        }

        this.certificateInfo = selected;
        this.initForm(selected);
      },
      error: (err) => {
        this.snackbarService.error(`Failed to load certificate template: ${err}`);
      }
    })
  }

  private initForm(cert: CertificateInfo): void {
    const controls: Record<string, any> = {};

    for (const field of cert.fields) {
      controls[field] = this.createControl(field);
    }

    controls['certificateType'] = [cert.type, Validators.required];
    
    controls['fee'] = [0, Validators.required];

    this.certificateForm = this.fb.group(controls);
  }

  private createControl(field: string) {
    switch (field) {
      case 'fullName':
        return ['', [Validators.required, Validators.minLength(5)]];

      case 'age':
        return ['', [Validators.required, Validators.min(1)]];

      case 'birthDate':
      case 'civilStatus':
      case 'purok':
      case 'purpose':
      case 'stayDuration':
      case 'gender':
      case 'placeOfBirth':
        return ['', Validators.required];

      default:
        return ['', Validators.required];
    }
  }

  getInputType(field: string): string {
    if (field === 'birthDate') return 'date';
    if (field === 'age') return 'number';
    return 'text';
  }

  isSelectField(field: string): boolean {
    return ['civilStatus', 'purok', 'gender'].includes(field);
  }

  getOptions(field: string): string[] {
    switch (field) {
      case 'civilStatus':
        return ['Single', 'Married', 'Widowed', 'Separated'];
      
      case 'purok':
        return ['Purok 1', 'Purok 2', 'Purok 3', 'Purok 4', 'Purok 5', 'Purok 6'];

      case 'gender':
        return ['Male', 'Female'];

      default:
        return [];
    }
  }

  isTextareaField(field: string): boolean {
    return field === 'purpose';
  }

  getLabel(field: string): string {
    return this.fieldLabels[field] ?? '';
  }

  getPlaceholder(field: string): string {
    return this.fieldPlaceholders[field] ?? '';
  }

  onSubmit(): void {
    if (this.certificateForm.invalid) {
      this.certificateForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const formData = this.certificateForm.value;

    this.certificateService.createCertificate(formData).subscribe({
      next: (response) => {
        this.snackbarService.success(
          `Application submitted! Ref: ${response.data.referenceNumber}` 
        )

        this.router.navigate(['/request-status']);
      },
      error: (err) => {
        this.snackbarService.error(`FAiled to submit application. Please try again`);
        this.isSubmitting = false;
      }
    });
  }

  clearForm(): void {
    this.certificateForm.reset({
      certificateType: this.certificateInfo?.type ?? 0,
      fee: 0
    });
  }

  isInvalid(controlName: string): boolean {
    const control = this.certificateForm.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }
}
