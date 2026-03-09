import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Resident } from '../../models/residents';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-create-edit-resident',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './create-edit-resident.component.html',
  styleUrl: './create-edit-resident.component.scss',
})
export class CreateEditResidentComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef);
  public data = inject<Resident | null>(MAT_DIALOG_DATA);

  residentForm: FormGroup;
  loading = false;
  previewUrl: string | null = null;

  constructor() {
    this.residentForm = this.fb.group({
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      birthDate: ['', Validators.required],
      civilStatus: ['Single'],
      purok: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      pictureUrl: ['']
    });
  }

  ngOnInit(): void {
    if (this.data) {
      this.residentForm.patchValue(this.data);
      this.previewUrl = this.data.pictureUrl;
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;

        this.residentForm.patchValue({ pictureUrl: this.previewUrl});
      };
      reader.readAsDataURL(file);
    }
  }

  openCamera() {
    alert('Camera module integration required');
  }

  onSubmit() {
    if (this.residentForm.valid) {
      this.loading = true,
      this.dialogRef.close(this.residentForm);
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
