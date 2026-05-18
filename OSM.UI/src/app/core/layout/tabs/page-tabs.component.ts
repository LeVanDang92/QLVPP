import { NgClass } from '@angular/common';
import { Component, inject } from '@angular/core';
import { PageTabService } from './page-tab.service';

@Component({
  selector: 'app-page-tabs',
  standalone: true,
  imports: [NgClass],
  templateUrl: './page-tabs.component.html',
  styleUrl: './page-tabs.component.scss',
})
export class PageTabsComponent {
  readonly tabService = inject(PageTabService);
}
