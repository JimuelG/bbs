import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BarangayOfficialService } from '../../../core/services/barangay-official.service';
import { BarangayOfficial } from '../../models/barangayOfficial';
import { range } from 'rxjs';
import { isAccessor } from 'typescript';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { MatIcon } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-official-add-edit-modal',
  imports: [
    MatIcon,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './official-add-edit-modal.component.html',
  styleUrl: './official-add-edit-modal.component.scss',
})
export class OfficialAddEditModalComponent implements OnInit {
  private fb = inject(FormBuilder);
  private barangayOfficialService = inject(BarangayOfficialService);
  private dialogRef = inject(MatDialogRef<OfficialAddEditModalComponent>);
  private snackbarService = inject(SnackbarService)

  public data = inject<BarangayOfficial | null>(MAT_DIALOG_DATA)

  officialForm = this.fb.group({
    id: [0],
    firstName: ['', Validators.required],
    middleName: ['', Validators.required],
    lastName: ['', Validators.required],
    position: ['', Validators.required],
    rank: [1, [Validators.required, Validators.min(1)]],
    isActive: [true],
  });

  ngOnInit() {
    if (this.data) {
      this.officialForm.patchValue(this.data);
    }
  }

  save() {
    if (this.officialForm.invalid) return;

    const val = this.officialForm.value;

    if (this.data) {
      this.barangayOfficialService.updateOfficial(this.data.id, val).subscribe({
        next: () => {
          this.dialogRef.close(true);
          this.snackbarService.success('Official updated successfully');
        },
        error: (err) => this.snackbarService.error(`Update failed" ${err.message || err}`),
      })
    } else {
      this.barangayOfficialService.createOfficial(val).subscribe({
        next: () => {
          this.dialogRef.close(true);
          this.snackbarService.success('Official created successfully');
        },
        error: (err) => this.snackbarService.error(`Creation failed" ${err.message || err}`),
      })
    }
  }

  upload(event: any, type: 'officials' | 'signature') {
    const file = event.target.files[0];

    if (file && this.data) {
      this.barangayOfficialService.uploadImage(this.data.id, type, file).subscribe({
        next: (updated) => {
          this.data = updated;
        },
        error: (err) => this.snackbarService.error(`Upload failed" ${err.message || err}`)
      })
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
