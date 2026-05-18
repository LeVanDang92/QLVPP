import { Component } from '@angular/core';
import { MobileMenuComponent } from '../menu/mobile-menu/mobile-menu.component';

@Component({
  selector: 'app-mobile-sidebar',
  standalone: true,
  imports: [MobileMenuComponent],
  templateUrl: './mobile-sidebar.component.html',
  styleUrl: './mobile-sidebar.component.scss',
})
export class MobileSidebarComponent {}
