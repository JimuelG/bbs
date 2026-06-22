import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../../core/services/dashboard.service';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { environment } from '../../../../environments/environment.development';
import { DashboardOfficial, RecentCertificatesRequest, UrgentConcern } from '../../../shared/models/dashboard';

@Component({
  selector: 'app-admin-dashboard',
  imports: [
    MatIcon,
    CommonModule,
    RouterLink
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private snackbardService = inject(SnackbarService);

  baseApiUrl = environment.apiUrl.replace('/api', '');
  
  stats = {
    totalResidents: 0,
    verifiedResidents: 0,
    pendingCertificates: 0,
    activeConcerns: 0,
    activeAnnouncements: 0
  };

  recentRequests: RecentCertificatesRequest[] = [];
  recentConcerns: UrgentConcern[] = [];
  officials: DashboardOfficial[] = [];

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard() {
    this.dashboardService.getDashboard().subscribe({
      next: (dashboard) => {
        this.stats = dashboard.stats;
        this.recentRequests = dashboard.recentRequest;
        this.recentConcerns = dashboard.recentConcern;
        this.officials = dashboard.officials;
      },
      error: () => {
        this.snackbardService.error('Failed to load dashboard data');
      }
    });
  }

  getOfficialImage(imageUrl?: string): string {
    if(!imageUrl || imageUrl === 'N/A') {
      return '/public/images/default-avatar.png';
    }

    return environment.apiUrl + imageUrl;
  }
}