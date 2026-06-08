import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ConcernService } from '../../core/services/concern.service';
import { SnackbarService } from '../../core/services/snackbar.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-concern',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatIconModule
  ],
  templateUrl: './concern.component.html',
  styleUrl: './concern.component.scss',
})
export class ConcernComponent {
  private fb = inject(FormBuilder);
  private concernService = inject(ConcernService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);

  concernForm: FormGroup;
  selectedFile: File | null = null;
  imagePreview: string | ArrayBuffer | null = null;
  loading = false;

  currentResident = 1;

  constructor() {
    this.concernForm = this.fb.group({
      title: ['', Validators.required],
      type: ['', Validators.required],
      purok: [''],
      incidentLocation: [''],
      description: ['', [Validators.required, Validators.minLength(10)]],
    })
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = e => this.imagePreview = reader.result;
      reader.readAsDataURL(file);
    }
  }

  removeImage() {
    this.selectedFile = null;
    this.imagePreview = null;
  }

  onSubmit() {
    if (this.concernForm.invalid) {
      this.concernForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const dto = {
      ...this.concernForm.value,
      residentId: this.currentResident
    };

    this.concernService.createConcern(dto).subscribe({
      next: (response) => {
        if (this.selectedFile && response.id) {
          this.concernService.uploadPhoto(response.id, this.selectedFile).subscribe({
            next: () => this.handleSuccess(),
            error: () => {
              this.loading = false;
              this.snackbarService.error('Report submitted, but photo upload failed.');
            }
          });
        } else {
          this.handleSuccess();
        }
      },
      error: () => {
        this.loading = false;
        this.snackbarService.error('Failed to submit report. Please try again.');
      }
    });
  }

  private handleSuccess() {
    this.loading = false;
    this.snackbarService.success('Your concern has been submitted successfully.');
    this.router.navigate(['/']);
  }
}
