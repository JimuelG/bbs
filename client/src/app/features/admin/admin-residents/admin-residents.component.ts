import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { AccountService } from '../../../core/services/account.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Resident } from '../../../shared/models/residents';
import { MatDialog } from '@angular/material/dialog';
import { IdPreviewModalComponent } from '../../../shared/components/id-preview-modal/id-preview-modal.component';
import { CreateEditResidentComponent } from '../../../shared/components/create-edit-resident/create-edit-resident.component';
import { RouterLink } from "@angular/router";
import { environment } from '../../../../environments/environment.development';
import { forkJoin, map, of, switchMap } from 'rxjs';

@Component({
  selector: 'app-admin-residents',
  imports: [
    MatIcon,
    RouterLink
],
  templateUrl: './admin-residents.component.html',
  styleUrl: './admin-residents.component.scss',
})
export class AdminResidentsComponent implements OnInit {
  baseUrl = environment.apiUrl;
  private accountService = inject(AccountService);
  private snackbarService = inject(SnackbarService);
  private dialog = inject(MatDialog);

  residents: Resident[] = [];
  loading = false;


  ngOnInit(): void {
    this.loadResidents();
  }

  loadResidents() {
    this.loading = true;
    this.accountService.getAllResidents().subscribe({
      next: (data) => {
        this.residents = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  verifyResident(id: string) {
    this.accountService.verifyResident(id.toString()).subscribe({
      next: () => {
        this.snackbarService.success('Resident verified successfully');
        this.loadResidents();
      },
      error: () => {
        this.snackbarService.error('Failed to verify resident');
      }
    });
  }

  viewId(resident: Resident) {
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
        this.loadResidents();
      }
    });
  }

  verifiedResidents() {
    this.accountService.getVerifiedResident().subscribe({
      next: (data) => {
        this.residents = data
      }
    })
  }

  editResident(resident: Resident) {
    const dialogRef = this.dialog.open(CreateEditResidentComponent, {
      width: 'auto',
      maxWidth: '95vw',
      data: resident || null
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.loading = true;

      const uploadTasks: any = {};

      uploadTasks.idUrl = result.idFile
        ? this.accountService.uploadIdCard(result.email, result.idFile).pipe(map((res: any) => res.url))
        : of (result.idUrl);

      uploadTasks.pictureUrl = result.file
        ? this.accountService.uploadProfilePicture(result.email, result.file).pipe(map((res: any) => res.url))
        : of(result.pictureUrl);
      
      forkJoin(uploadTasks).pipe(
        switchMap((urls: any) => {
          const finalPayload = {
            ...result,
            idUrl: urls.idUrl,
            pictureUrl: urls.pictureUrl
          }

          delete finalPayload.file;
          delete finalPayload.idFile;

          return this.accountService.updateResident(resident.id, finalPayload);
        })
      ).subscribe({
        next: () => {
          this.snackbarService.success('Resident account updated successfully!');
          this.loadResidents();
          this.loading = false;
        },
        error: () => {
          this.snackbarService.error('Failed to update resident profile or upload assets.');
          this.loading = false;
        }
      })
    })
  }

  createResident() {
    const dialogRef = this.dialog.open(CreateEditResidentComponent, {
      width: 'auto',
      maxWidth: '95vw',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.loading = true;

      this.accountService.register(result).pipe(
        switchMap((response) => {
          const uploadRequests = [];

          if (result.idFile) {
            uploadRequests.push(
              this.accountService.uploadIdCard(result.email, result.idFile)
            );
          }

          if (result.file) {
            uploadRequests.push(
              this.accountService.uploadProfilePicture(result.email, result.file)
            );
          }

          return uploadRequests.length > 0 ? forkJoin(uploadRequests) : of(null);
        })
      ).subscribe({
        next: () => {
          this.snackbarService.success('Resident account created and profile assets uploaded successfully!');
          this.loadResidents();
          this.loading = false;
        },
        error: (err) => {
          this.snackbarService.error('Account registration failed or asset upload failed.');
          this.loading = false;
        }
      })

    })
  }
}
