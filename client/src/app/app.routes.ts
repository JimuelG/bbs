import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { AnnouncementComponent } from './features/announcement/announcement.component';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { AdminDashboardComponent } from './features/admin/admin-dashboard/admin-dashboard.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { AdminCertificatesComponent } from './features/admin/admin-certificates/admin-certificates.component';
import { AdminAnnouncementsComponent } from './features/admin/admin-announcements/admin-announcements.component';
import { AdminResidentsComponent } from './features/admin/admin-residents/admin-residents.component';
import { LoginComponent } from './features/account/login/login.component';
import { RegisterComponent } from './features/account/register/register.component';

export const routes: Routes = [
    {
        path: 'admin',
        component: AdminLayoutComponent,
        children: [
            {path: '', component: AdminDashboardComponent},
            {path: 'dashboard', component: AdminDashboardComponent},
            {path: 'announcements', component: AdminAnnouncementsComponent},
            {path: 'certificates', component: AdminCertificatesComponent},
            {path: 'residents', component: AdminResidentsComponent},

        ]
    },
    {
        path: '',
        component: MainLayoutComponent,
        children: [
            {path: '', component: HomeComponent},
            {path: 'announcement', component: AnnouncementComponent},
            {path: 'account/login', component: LoginComponent},
            {path: 'account/register', component: RegisterComponent},
            {path: '**', redirectTo: '', pathMatch: 'full'},
        ]
    }
]