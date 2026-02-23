import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-id-preview-modal',
  imports: [
    MatIcon,
    MatDialogModule
  ],
  templateUrl: './id-preview-modal.component.html',
  styleUrl: './id-preview-modal.component.scss',
})
export class IdPreviewModalComponent {
  data = inject(MAT_DIALOG_DATA);
  baseUrl = "http://localhost:5001";
}
