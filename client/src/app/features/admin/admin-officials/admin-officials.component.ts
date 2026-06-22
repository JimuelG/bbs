import { Component, inject, OnInit, signal } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { BarangayOfficial } from '../../../shared/models/barangayOfficial';
import { BarangayOfficialService } from '../../../core/services/barangay-official.service';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { OfficialAddEditModalComponent } from '../../../shared/components/official-add-edit-modal/official-add-edit-modal.component';
import { environment } from '../../../../environments/environment.development';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { RouterLink } from '@angular/router';
import { Pagination } from '../../../shared/models/pagination';
import { BarangayOfficialParams } from '../../../shared/models/barangayOfficialParams';

@Component({
  selector: 'app-admin-officials',
  imports: [
    MatIcon,
    ReactiveFormsModule,
    CommonModule,
    RouterLink
  ],
  templateUrl: './admin-officials.component.html',
  styleUrl: './admin-officials.component.scss',
})
export class AdminOfficialsComponent implements OnInit {
  baseApiUrl = environment.apiUrl;
  private barangayOfficialService = inject(BarangayOfficialService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  officials: BarangayOfficial[] = [];
  officialPagination?: Pagination<BarangayOfficial>;
  officialParams = new BarangayOfficialParams();
  totalCount = 0;
  pageSizeOptions = [10,20,30];
  

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
    this.barangayOfficialService.getAllOfficials(this.officialParams).subscribe({
      next: (response) => {
        this.officialPagination = response;
        this.officials = response.data;
        this.totalCount = response.count;
      }
    })
  }

  onPageChange(event: any) {
    this.officialParams.pageIndex = event.pageIndex + 1;
    this.officialParams.pageSize = event.pageSize;
    this.loadOfficials();
  }

  updatePage(newPageIndex: number) {
    this.officialParams.pageIndex = newPageIndex;
    this.loadOfficials();
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.officialParams.pageSize = parseInt(select.value);
    this.loadOfficials();
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }

  onSearchChange() {
    this.officialParams.pageIndex = 1;
    this.loadOfficials();
  }

  onSortChange() {
    this.officialParams.pageIndex = 1;
    this.loadOfficials();
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

    getOfficeImage(officeImage: string): string {
    
    if (!officeImage || officeImage === "N/A") {
      return '/public/images/default-avatar.png';
    }
    
    return this.baseApiUrl + officeImage;
  }
}
