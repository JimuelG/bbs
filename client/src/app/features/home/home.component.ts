import { DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [
    MatIcon,
    DatePipe,
    RouterLink
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {

  latestNews = [
    { id: 1, title: 'Barangay Health Mission 2026', category: 'Health', date: new Date() },
    { id: 2, title: 'New Curfew Guidelines', category: 'Security', date: new Date() },
    { id: 3, title: 'Summer Sports Fest Registration', category: 'Events', date: new Date() },
    { id: 4, title: 'Waste Management Update', category: 'Environment', date: new Date() },
  ];

  constructor() {}

  ngOnInit(): void {}
}
