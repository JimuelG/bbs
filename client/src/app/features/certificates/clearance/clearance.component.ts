import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';
import { CertificatesService } from '../../../core/services/certificates.service';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-clearance',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIcon,
  ],
  templateUrl: './clearance.component.html',
  styleUrl: './clearance.component.scss',
})
export class ClearanceComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private certificateService = inject(CertificatesService);
  private snackbarService = inject(SnackbarService);
  certificateForm!: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
  ) {}

  ngOnInit(): void {
    
    this.initForm();
  }

  private initForm(): void {
    const typeId = this.route.snapshot.paramMap.get('id');
    this.certificateForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(5)]],
      address: ['', Validators.required],
      birthDate: ['', Validators.required],
      certificateType: [typeId ? parseInt(typeId) : 0, Validators.required], // 0 = Barangay Clearance
      purpose: ['', Validators.required],
      fee: [0, Validators.required], // Default fee
      civilStatus: ['Single', Validators.required],
      stayDuration: ['', Validators.required],
      purok: ['', Validators.required]
    });
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
      // Show success notification
      this.snackbarService.success(
        `Application Submitted! Ref: ${response.data.referenceNumber}`
      )

      // Open the generated PDF in a new tab if it exists
      if (response.pdfUrl) {
        window.open(response.pdfUrl, '_blank');
      }

      // Navigate to a status or "Thank You" page
      this.router.navigate(['/request-status']);
    },
    error: (err) => {
      console.error('Submission Error:', err);
      this.snackbarService.error('Failed to submit application. Please try again.');
      this.isSubmitting = false;
    }
  });
  }

  // Helper to check validation in HTML
  isInvalid(controlName: string): boolean {
    const control = this.certificateForm.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }
}
