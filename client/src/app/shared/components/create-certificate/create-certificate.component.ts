import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';
import { CertificatesService } from '../../../core/services/certificates.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Certificate } from '../../models/certificate';
import { CdkVirtualScrollableElement } from "@angular/cdk/scrolling";

@Component({
  selector: 'app-create-certificate',
  imports: [
    MatIcon,
    ReactiveFormsModule,
    CommonModule,
    CdkVirtualScrollableElement
],
  templateUrl: './create-certificate.component.html',
  styleUrl: './create-certificate.component.scss',
})
export class CreateCertificateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private snackBar = inject(SnackbarService);
  private dialogRef = inject(MatDialogRef);
  private certificateService = inject(CertificatesService);

  public data = inject<Certificate | null>(MAT_DIALOG_DATA)

  certificateForm: FormGroup;
  loading = false;
  isEditing = false;

  ngOnInit(): void {
    if (this.data) {
      this.certificateForm.patchValue(this.data);
      this.certificateForm.disable();
    }
  }

  constructor() {
    this.certificateForm = this.fb.group({
      fullName: ['', Validators.required],
      address: ['', Validators.required],
      certificateType: [0, [Validators.required, Validators.min(0)]],
      purpose: ['', Validators.required],
      fee: [0, [Validators.required, Validators.min(0)]],
      birthDate: ['', Validators.required],
      civilStatus: ['', Validators.required],
      purok: ['', Validators.required],
      stayDuration: ['', Validators.required]
    })
  }

  onSubmit(): void {
    if (this.certificateForm.valid) {
      this.loading = true;

      const payload = {
        ...this.certificateForm.value,
        certificateType: Number(this.certificateForm.value.certificateType ?? 0),
        fee: Number(this.certificateForm.value.fee ?? 0)
      }

      this.certificateService.createCertificate(payload).subscribe({
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

  onEdit() {
    this.certificateForm.enable();
    this.isEditing = true;
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
