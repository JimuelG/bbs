import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AccountService } from '../../../../core/services/account.service';
import { Resident } from '../../../../shared/models/residents';
import { MatIcon } from '@angular/material/icon';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { CreateEditResidentComponent } from '../../../../shared/components/create-edit-resident/create-edit-resident.component';
import { SnackbarService } from '../../../../core/services/snackbar.service';

@Component({
  selector: 'app-admin-residents-detail',
  imports: [
    MatIcon,
    DatePipe,
    RouterLink
  ],
  templateUrl: './admin-residents-detail.component.html',
  styleUrl: './admin-residents-detail.component.scss',
})
export class AdminResidentsDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private dialog = inject(MatDialog);
  private snackbarService = inject(SnackbarService);


  resident: Resident | null = null;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.accountService.getResidentDetails(id).subscribe(res => {
        this.resident = res;
      })
    }
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

}
