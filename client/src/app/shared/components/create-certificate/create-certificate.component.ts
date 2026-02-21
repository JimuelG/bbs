import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { CertificatesService } from '../../../core/services/certificates.service';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-create-certificate',
  imports: [
    MatIcon,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './create-certificate.component.html',
  styleUrl: './create-certificate.component.scss',
})
export class CreateCertificateComponent {
  private fb = inject(FormBuilder);
  private snackBar = inject(SnackbarService);
  private dialogRef = inject(MatDialogRef);
  private certificateService = inject(CertificatesService);

  certificateForm: FormGroup;
  loading = false;

  constructor() {
    this.certificateForm = this.fb.group({
      fullName: ['', Validators.required],
      address: ['', Validators.required],
      certificateType: [3, Validators.required],
      purpose: ['', Validators.required],
      fee: [0, [Validators.required, Validators.min(0)]]
    })
  }

  onSubmit(): void {
    if (this.certificateForm.valid) {
      this.loading = true;

      this.certificateService.createCertificate(this.certificateForm.value).subscribe({
        next: (response) => {
          this.loading = false;
          this.dialogRef.close(response);
        },
        error: (err) => {
          this.loading = false;
          this.snackBar.error(`Certificate creation failed: ${err}`)
        }
      })
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
