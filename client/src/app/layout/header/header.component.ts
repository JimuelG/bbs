import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from "@angular/router";
import { MatAnchor, MatButton } from "@angular/material/button";

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    MatAnchor,
    MatButton
],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {

}
