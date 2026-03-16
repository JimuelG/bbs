import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BarangayOfficialService } from '../../../../core/services/barangay-official.service';
import { BarangayOfficial } from '../../../../shared/models/barangayOfficial';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-admin-officials-details',
  imports: [
    MatIcon,
    RouterLink
  ],
  templateUrl: './admin-officials-details.component.html',
  styleUrl: './admin-officials-details.component.scss',
})
export class AdminOfficialsDetailsComponent implements OnInit{
  private route = inject(ActivatedRoute);
  private officialService = inject(BarangayOfficialService);

  official: BarangayOfficial | null = null;

  ngOnInit(): void {
    this.loadOfficial();
  }

  loadOfficial() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.officialService.getOfficialById(+id).subscribe(data => {
        this.official = data;
      });
    }
  }
}
