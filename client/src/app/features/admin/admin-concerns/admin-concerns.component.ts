import { Component, inject, OnInit } from '@angular/core';
import { ConcernService } from '../../../core/services/concern.service';
import { Concern } from '../../../shared/models/concern';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { Pagination } from '../../../shared/models/pagination';
import { ConcernParms } from '../../../shared/models/concernParams';
import { environment } from '../../../../environments/environment.development';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-concerns',
  imports: [
    CommonModule,
    DatePipe,
    RouterLink,
    MatIconModule,
    FormsModule
  ],
  templateUrl: './admin-concerns.component.html',
  styleUrl: './admin-concerns.component.scss',
})
export class AdminConcernsComponent implements OnInit{
  private concernService = inject(ConcernService);
  private snackbarService = inject(SnackbarService);
  
  concerns: Concern[] = [];
  concernPagination?: Pagination<Concern>;
  concernParams = new ConcernParms();
  totalCount = 0;
  pageSizeOptions = [10,20,30];
  baseApiUrl = environment.apiUrl;

  ngOnInit() {
    this.loadConcerns();
  }

  loadConcerns() {
    this.concernService.getAllConcerns(this.concernParams).subscribe({
      next: (response) => {
        this.concernPagination = response;
        this.concerns = response.data;
        this.totalCount = response.count;
      }
    })
  }

  onPageChange(event: any) {
    this.concernParams.pageIndex = event.pageIndex + 1;
    this.concernParams.pageSize = event.pageSize;
    this.loadConcerns();
  }

  updatePage(newPageIndex: number) {
    this.concernParams.pageIndex = newPageIndex;
    this.loadConcerns();
  }

  onPageSizeChange(event: Event) {
    const select = event.target as HTMLSelectElement;
    this.concernParams.pageSize = parseInt(select.value);
    this.loadConcerns();
  }

  mathMin(a: number, b: number): number {
    return Math.min(a, b);
  }

  onSearchChange() {
    this.concernParams.pageIndex = 1;
    this.loadConcerns();
  }

  onSortChange() {
    this.concernParams.pageIndex = 1;
    this.loadConcerns();
  }

  onStatusChange() {
    this.concernParams.pageIndex = 1;
    this.loadConcerns();
  }

  onPriorityChange() {
    this.concernParams.pageIndex = 1;
    this.loadConcerns();
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
