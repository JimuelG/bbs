import { Component, inject, OnInit } from '@angular/core';
import { ConcernService } from '../../../core/services/concern.service';
import { Concern } from '../../../shared/models/concern';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-admin-concerns',
  imports: [
    CommonModule,
    DatePipe,
    RouterLink,
    MatIconModule
  ],
  templateUrl: './admin-concerns.component.html',
  styleUrl: './admin-concerns.component.scss',
})
export class AdminConcernsComponent implements OnInit{
  private concernService = inject(ConcernService);
  concerns: Concern[] = [];
  

  ngOnInit() {
    this.concernService.getConcerns().subscribe({
      next: (data) => {
        this.concerns = data;
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
