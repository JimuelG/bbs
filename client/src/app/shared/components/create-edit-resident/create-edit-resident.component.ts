import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Resident } from '../../models/residents';

@Component({
  selector: 'app-create-edit-resident',
  imports: [],
  templateUrl: './create-edit-resident.component.html',
  styleUrl: './create-edit-resident.component.scss',
})
export class CreateEditResidentComponent {
  private dialogRef = inject(MatDialogRef);

  public data = inject<Resident | null>(MAT_DIALOG_DATA);

  
}
