import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatCard } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { BarangayOfficial } from '../../../shared/models/barangayOfficial';
import { BarangayOfficialService } from '../../../core/services/barangay-official.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { OfficialAddEditModalComponent } from '../../../shared/components/official-add-edit-modal/official-add-edit-modal.component';
import { environment } from '../../../../environments/environment.development';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-admin-officials',
  imports: [
    MatIcon,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './admin-officials.component.html',
  styleUrl: './admin-officials.component.scss',
})
export class AdminOfficialsComponent implements OnInit {
  baseUrl = "http://localhost:5001";
  private barangayOfficialService = inject(BarangayOfficialService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  officials = signal<BarangayOfficial[]>([]);

  ngOnInit() {
    this.loadOfficials();
  }

  openOfficialModal(official?: BarangayOfficial) {
    const dialogRef = this.dialog.open(OfficialAddEditModalComponent, {
      width: '500px',
      data: official || null,
    })

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadOfficials();
    })
  }

  loadOfficials() {
    this.barangayOfficialService.getBarangayOfficials().subscribe({
      next: (officials) => {
        this.officials.set(officials);
      },
      error: (error) => {
        console.error('Error loading officials:', error);
      }
    });
  }

  onDelete(id: number) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '400px',
      data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete this official? This action cannot be undone.'
      }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.barangayOfficialService.deleteOfficial(id).subscribe({
          next: () => {
            this.loadOfficials();
            this.snackbarService.success('Official deleted successfully');
          },
          error: (error) => {
            this.snackbarService.error(`Failed to delete official: ${error.message || error}`);
          }
        });
      }
    });
  }

}
