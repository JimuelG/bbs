import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Resident } from '../../models/residents';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTooltip } from '@angular/material/tooltip';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-create-edit-resident',
  imports: [
    ReactiveFormsModule,
    MatTooltip
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
  idPreviewUrl: string | null = null;
  selectedFile: File | null = null;
  selectedIdFile: File | null = null;
  baseApiUrl = environment.apiUrl;
  isLocalPreview = false;
  isLocalIdPreview = false;

  constructor() {
    this.residentForm = this.fb.group({
      firstName: ['', Validators.required],
      middleName: [''],
      lastName: ['', Validators.required],
      email: ['', Validators.required],
      birthDate: ['', Validators.required],
      civilStatus: ['Single'],
      purok: ['', Validators.required],
      idUrl: [''],
      phoneNumber: ['', [Validators.required, Validators.pattern('^[0-9]*$')]],
      pictureUrl: ['']
    });
  }

  ngOnInit(): void {
    if (this.data) {
      const formattedBirthDate = this.data.birthDate
        ? this.data.birthDate.split('T')[0]
        : '';
      
      this.residentForm.patchValue({
        ...this.data,
        birthDate: formattedBirthDate
      });

      this.previewUrl = this.data.pictureUrl;
      this.idPreviewUrl = this.data.idUrl;
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;
        this.isLocalPreview = true;
      };
      reader.readAsDataURL(file);
    }
  }

  onIdFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedIdFile = file;

      const reader = new FileReader();
      reader.onload = () => {
        this.idPreviewUrl = reader.result as string;
        this.isLocalIdPreview = true;
      };
      reader.readAsDataURL(file);
    }
  }

  openCamera() {
    alert('Camera module integration required');
  }

  onSubmit() {
    if (this.residentForm.valid) {
      this.loading = true;
      this.dialogRef.close({
        ...this.residentForm.value,
        file: this.selectedFile,
        idFile: this.selectedIdFile
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }
}
