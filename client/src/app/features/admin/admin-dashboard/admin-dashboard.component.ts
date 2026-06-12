import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

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
  
  stats = {
    totalResidents: 1245,
    verifiedResidents: 980,
    pendingCertificates: 12,
    activeConcerns: 5,
    activeAnnouncements: 3
  };

  // Mock Data Arrays for Dashboard Widgets
  recentRequests = [
    { id: 'REQ-001', name: 'Juan Dela Cruz', type: 'Barangay Clearance', status: 'Pending', date: 'Today, 9:00 AM' },
    { id: 'REQ-002', name: 'Maria Santos', type: 'Certificate of Indigency', status: 'Processing', date: 'Today, 8:30 AM' },
    { id: 'REQ-003', name: 'Pedro Reyes', type: 'Business Clearance', status: 'Pending', date: 'Yesterday' }
  ];

  recentConcerns = [
    { id: 'CON-101', title: 'Broken Streetlight', purok: 'Purok 1', priority: 'High', status: 'Unresolved' },
    { id: 'CON-102', title: 'Noise Complaint', purok: 'Purok 3', priority: 'Medium', status: 'In Progress' }
  ];

  officials = [
    { name: 'Hon. Roberto Garcia', position: 'Punong Barangay', imageUrl: null },
    { name: 'Hon. Elena Castro', position: 'Kagawad - Health', imageUrl: null },
    { name: 'Hon. Miguel Torres', position: 'Kagawad - Peace & Order', imageUrl: null }
  ];

  ngOnInit(): void {
    // Inject your services here to load real data
    // this.loadDashboardStats();
  }
}
