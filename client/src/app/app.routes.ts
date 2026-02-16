import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { AnnouncementComponent } from './features/announcement/announcement.component';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { AdminCertificatesComponent } from './features/admin/admin-certificates/admin-certificates.component';
import { AdminAnnouncementsComponent } from './features/admin/admin-announcements/admin-announcements.component';

export const routes: Routes = [
    {
        path: 'admin',
        component: AdminLayoutComponent,
        children: [
            {path: '', component: AdminDashboardComponent},
            {path: 'dashboard', component: AdminDashboardComponent},
            {path: 'announcements', component: AdminAnnouncementsComponent},
            {path: 'certificates', component: AdminCertificatesComponent},

        ]
    },
    {
        path: '',
        component: MainLayoutComponent,
        children: [
            {path: '', component: HomeComponent},
            {path: 'announcement', component: AnnouncementComponent},
            {path: '**', redirectTo: '', pathMatch: 'full'},
        ]
    }
]