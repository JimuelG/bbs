import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { StaffService } from '../../../core/services/staff.service';
import { Staff } from '../../models/staff';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-edit-staff',
  imports: [
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './create-edit-staff.component.html',
  styleUrl: './create-edit-staff.component.scss',
})
export class CreateEditStaffComponent implements OnInit {
  private fb = inject(FormBuilder);
  private snackbarService = inject(SnackbarService);
  private dialogRef = inject(MatDialogRef);
  private staffService = inject(StaffService);

  public data = inject<Staff | null>(MAT_DIALOG_DATA);

  staffForm: FormGroup;
  loading = false;

  ngOnInit(): void {
    if (this.data) {
      this.staffForm.patchValue(this.data);
      this.staffForm.disable();
    }
  }

  constructor() {
    this.staffForm = this.fb.group({
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      position: ['', Validators.required],
      birthDate: ['', Validators.required],
      email: ['']
    })
  }

 onSubmit(): void {
  if (this.staffForm.invalid) {
    this.staffForm.markAllAsTouched();
    return;
  }

  this.loading = true;
  const staffData = this.staffForm.value;

  if (this.data && this.data.id) {
    // Scenario: UPDATE EXISTING STAFF
    this.staffService.updateStaff(this.data.id, staffData).subscribe({
      next: (response: any) => {
        this.snackbarService.success(response.message || 'Staff updated successfully');
        this.dialogRef.close(true); // Return true to trigger list refresh
      },
      error: (err) => {
        this.loading = false;
        this.snackbarService.error('Failed to update staff member');
      }
    });
  } else {
    // Scenario: CREATE NEW STAFF
    this.staffService.createStaff(staffData).subscribe({
      next: (response: any) => {
        this.snackbarService.success(response.message || 'Staff created successfully');
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.loading = false;
        this.snackbarService.error('Failed to create staff member');
      }
    });
  }
}

  onCancel(): void {
    this.dialogRef.close();
  }

  onEnableEdit(): void {
    this.staffForm.enable();
  }

  onDelete(id: number) {
    
  }
}
