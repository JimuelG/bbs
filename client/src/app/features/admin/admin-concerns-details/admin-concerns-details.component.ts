import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ConcernService } from '../../../core/services/concern.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Concern } from '../../../shared/models/concern';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-admin-concerns-details',
  imports: [
    CommonModule,
    RouterLink,
    MatIconModule,
    ReactiveFormsModule
  ],
  templateUrl: './admin-concerns-details.component.html',
  styleUrl: './admin-concerns-details.component.scss',
})
export class AdminConcernsDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private concernService = inject(ConcernService);
  private fb = inject(FormBuilder);
  private snackbarService = inject(SnackbarService);


  concern: Concern | null = null;
  updateForm!: FormGroup;
  officials: any[] = [];
  loading = false;
  baseApiUrl = environment.apiUrl;

  ngOnInit() {
    this.initForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadConcern(id);
    }
  }

  initForm() {
    this.updateForm = this.fb.group({
      status: ['', Validators.required],
      assignedOfficialId: [null],
      resolutionRemarks: ['']
    });
  }

  loadConcern(id: string) {
    this.concernService.getConcern(id).subscribe({
      next: data => {
        this.concern = data;
        this.updateForm.patchValue({
          status: this.concern.status,
          assignedOfficialId: this.concern.assignedOfficialId,
          resolutionRemarks: this.concern.resolutionRemarks
        });
      }
    });
  }

  onSubmit() {
    if (this.updateForm.invalid || !this.concern) return;

    this.loading = true;
    this.concernService.updateConcern(this.concern.id, this.updateForm.value).subscribe({
      next: () => {
        this.loading = false;
        this.snackbarService.success('Concern updated successfully');
        this.loadConcern(this.concern!.id.toString());
      },
      error: () => {
        this.loading = false;
        this.snackbarService.error('Failed to update concern');
      }
    });
  }

  getStatusClass(status: string): string {
    switch(status) {
      case 'Pending': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'UnderReview': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'InProgress': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'Resolved': return 'bg-green-100 text-green-800 border-green-200';
      case 'Dismissed': return 'bg-gray-100 text-gray-800 border-gray-200';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}
