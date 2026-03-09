import { Component, inject, OnInit } from '@angular/core';
import { StaffService } from '../../../core/services/staff.service';
import { Staff } from '../../../shared/models/staff';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { CreateEditStaffComponent } from '../../../shared/components/create-edit-staff/create-edit-staff.component';

@Component({
  selector: 'app-admin-staffs',
  imports: [
    DatePipe
  ],
  templateUrl: './admin-staffs.component.html',
  styleUrl: './admin-staffs.component.scss',
})
export class AdminStaffsComponent implements OnInit{
  private staffService = inject(StaffService);
  private snackbarService = inject(SnackbarService);
  private dialog = inject(MatDialog);

  staffs: Staff[] = [];

  ngOnInit(): void {
    this.loadStaffs();
  }

  loadStaffs() {
    this.staffService.getStaffs().subscribe({
      next: (response) => {
        this.staffs = response
      }
    })
  }

  onEdit(staff: Staff) {
    const dialogRef = this.dialog.open(CreateEditStaffComponent, {
      width: 'auto',
      maxWidth: '95vw',
      data: staff || null
    });
  }

  openStaffModal(): void {
    const dialogRef = this.dialog.open(CreateEditStaffComponent, {
      width: 'auto',
      maxWidth: '95vw',

    })

    dialogRef.afterClosed().subscribe(result => {
      if(result) {
        this.loadStaffs();
      }
    })
  }
  
}
