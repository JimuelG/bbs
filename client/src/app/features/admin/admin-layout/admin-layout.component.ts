import { Component, inject } from '@angular/core';
import { MatProgressBar } from '@angular/material/progress-bar';
import { BusyService } from '../../../core/services/busy.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { MatList, MatListItem } from '@angular/material/list';
import { RouterOutlet, RouterLink } from '@angular/router';

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
export class AdminLayoutComponent {
  busyService = inject(BusyService);

  
}
