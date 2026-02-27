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
],
  templateUrl: './create-certificate.component.html',
  styleUrl: './create-certificate.component.scss',
})
export class CreateCertificateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private snackbarService = inject(SnackbarService);
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
      id: [''],
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

      const formValue = this.certificateForm.getRawValue();

      const payload = {
        ...formValue,
        certificateType: Number(this.certificateForm.value.certificateType ?? 0),
        fee: Number(this.certificateForm.value.fee ?? 0)
      }
     
      const request$ = this.data?.id
        ? this.certificateService.updateCertificate(this.data.id, payload)
        : this.certificateService.createCertificate(payload);

      request$.subscribe({
        next: (response) => {
          this.loading = false;

          const action = this.data?.id ? 'updated' : 'issued';
          this.snackbarService.success(`Certificate ${action} successfully!`);

          this.dialogRef.close(response);
        },
        error: (err) => {
          this.loading = false;
          const action = this.data?.id ? 'updated' : 'creation';
          this.snackbarService.error(`Certificate ${action} failed. Please check your inputs`);
        }
      })
    }
  }

  onEdit(event: Event): void {
    event.preventDefault();
    this.isEditing = true;
    this.certificateForm.enable();
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
