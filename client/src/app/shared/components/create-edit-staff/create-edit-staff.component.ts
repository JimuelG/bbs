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

  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onEnableEdit(): void {
    this.staffForm.enable;
  }

  onDelete(id: number) {
    
  }
}
