import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

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
  private dialogRef = inject(MatDialogRef);

  certificateForm: FormGroup;
  loading = false;

  constructor() {
    this.certificateForm = this.fb.group({
      residentName: ['', Validators.required],
      certificateType: ['clearance', Validators.required],
      purpose: ['', Validators.required],
      cedulaNumber: [''],
      cedulaDate: ['']
    })
  }

  onSubmit(): void {
    if (this.certificateForm.valid) {
      this.loading = true;

      setTimeout(() => {
        this.loading = false;
        this.dialogRef.close(this.certificateForm.value);
      }, 1500);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
