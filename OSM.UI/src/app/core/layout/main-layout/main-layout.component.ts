import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { MobileSidebarComponent } from '../mobile-sidebar/mobile-sidebar.component';
import { PageTabsComponent } from '../tabs/page-tabs.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [HeaderComponent, MobileSidebarComponent, PageTabsComponent, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  closeMobileSidebar(): void {
    document.documentElement.classList.remove('menu-opened');
    document.body.classList.remove('menu-opened');
    document.querySelector('.main-wrapper')?.classList.remove('slide-nav');
    document.querySelector('.sidebar-overlay')?.classList.remove('opened');
  }
}
