import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AccountService } from '../../../../core/services/account.service';
import { Resident } from '../../../../shared/models/residents';
import { MatIcon } from '@angular/material/icon';
import { CommonModule, DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { CreateEditResidentComponent } from '../../../../shared/components/create-edit-resident/create-edit-resident.component';
import { SnackbarService } from '../../../../core/services/snackbar.service';
import { environment } from '../../../../../environments/environment.development';
import { IdPreviewModalComponent } from '../../../../shared/components/id-preview-modal/id-preview-modal.component';
import { ResidentRecord } from '../../../../shared/models/residentRecord';

@Component({
  selector: 'app-admin-residents-detail',
  imports: [
    MatIcon,
    DatePipe,
    RouterLink,
    CommonModule
  ],
  templateUrl: './admin-residents-detail.component.html',
  styleUrl: './admin-residents-detail.component.scss',
})
export class AdminResidentsDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);

  baseApiUrl = environment.apiUrl;

  resident: Resident | null = null;
  residentRecords: ResidentRecord[] = [];

  private residentId: string | null = null;

  ngOnInit(): void {
    this.residentId = this.route.snapshot.paramMap.get('id');
    
    if (this.residentId) {
      this.loadResident();
      this.loadResidentRecords();
    }

  }

  loadResident() {
    if (!this.residentId) return;

    this.accountService.getResidentDetails(this.residentId).subscribe({
      next: (res) => {
        this.resident = res;
      },
      error: () => {
        this.snackbarService.error('Failed to load resident details');
      }
    })
  }

  loadResidentRecords() {
    if (!this.residentId) return;

    this.accountService.getResidentRecords(this.residentId).subscribe({
      next: (records) => {
        this.residentRecords = records;
      },
      error: () => {
        this.snackbarService.error('Failed to load resident records');
      }
    })
  }

  editResident(resident: Resident) {
    const dialogRef = this.dialog.open(CreateEditResidentComponent, {
      width: 'auto',
      maxWidth: '95vw',
      data: resident || null
    })
  }

  verifyResident(id: string) {
    this.accountService.verifyResident(id.toString()).subscribe({
      next: () => {
        this.snackbarService.success('Resident verified successfully');
      },
      error: () => {
        this.snackbarService.error('Failed to verify resident');
      }
    });
  }

  viewSubmittedId(resident: Resident) {
    const dialogRef = this.dialog.open(IdPreviewModalComponent, {
      data: {
        id: `${resident.id}`,
        idUrl: `${resident.idUrl}`,
        name: `${resident.firstName} ${resident.lastName}`,
        isVerified: resident.isIdVerified
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'verified') {
        this.verifyResident(resident.id);
      }
    });
  }

  getProfileImage(): string {
    if (this.resident?.pictureUrl) {
      return this.baseApiUrl + this.resident.pictureUrl;
    }
    
    return '/public/images/default-avatar.png';
  }

}
