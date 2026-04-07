import { Component, inject, OnInit, signal } from '@angular/core';
import { MatProgressBar } from '@angular/material/progress-bar';
import { BusyService } from '../../../core/services/busy.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { MatList, MatListItem } from '@angular/material/list';
import { RouterOutlet, RouterLink } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-admin-layout',
  imports: [
    MatProgressBar,
    MatIcon,
    MatButtonModule,
    MatList,
    MatListItem,
    RouterOutlet,
    RouterLink
],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent implements OnInit{
  private breakpointObserver = inject(BreakpointObserver);
  busyService = inject(BusyService);

  isMobile = signal(false);
  isSidebarVisible = signal(true);

  ngOnInit(): void {
    this.breakpointObserver.observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
      .subscribe(result => {
        const isSmall = result.matches;
        this.isMobile.set(isSmall);

        this.isSidebarVisible.set(!isSmall);
      });
  }

  toggleSidebar() {
    this.isSidebarVisible.update(v => !v);
  }

  navLinks = [
    { path: '/admin/dashboard', icon: 'dashboard', label: 'Dashboard' },
    { path: '/admin/barangay-officials', icon: 'gavel', label: 'Barangay Officials' },
    // { path: '/admin/staffs', icon: 'groups', label: 'Barangay Staffs' },
    { path: '/admin/residents', icon: 'group', label: 'Residents' },
    { path: '/admin/announcements', icon: 'campaign', label: 'Announcements'},
    { path: '/admin/certificates', icon: 'description', label: 'Certificates' },
  ];
}
