import { Component } from '@angular/core';
import { HeaderComponent } from "../header/header.component";
import { FooterComponent } from "../footer/footer.component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  imports: [HeaderComponent, RouterOutlet, FooterComponent],
  template: `
  <app-header></app-header>
    <main>
        <router-outlet></router-outlet>
    </main>
  <app-footer></app-footer>
  `,
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {

}
