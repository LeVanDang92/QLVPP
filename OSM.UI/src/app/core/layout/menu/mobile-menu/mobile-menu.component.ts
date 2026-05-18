import { NgTemplateOutlet } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { PageTabService } from '../../tabs/page-tab.service';
import { APP_MENU_SECTIONS } from '../menu.data';
import { MenuItem } from '../menu-item.model';

@Component({
  selector: '[appMobileMenu]',
  standalone: true,
  imports: [NgTemplateOutlet],
  templateUrl: './mobile-menu.component.html',
  styleUrl: './mobile-menu.component.scss',
})
export class MobileMenuComponent {
  private readonly pageTabs = inject(PageTabService);
  private readonly openedIdsSignal = signal<Set<string>>(new Set<string>());

  readonly sections = APP_MENU_SECTIONS;
  readonly activePath = this.pageTabs.activePath;

  hasChildren(item: MenuItem): boolean {
    return !!item.children?.length;
  }

  isOpen(item: MenuItem): boolean {
    return this.openedIdsSignal().has(item.id);
  }

  isActive(item: MenuItem): boolean {
    if (item.path && this.activePath() === item.path) {
      return true;
    }

    return item.children?.some((child) => this.isActive(child)) ?? false;
  }

  submenuClass(level: number): string {
    if (level <= 0) {
      return 'submenu';
    }

    return level === 1 ? 'submenu submenu-two' : 'submenu submenu-two submenu-three';
  }

  menuArrowClass(level: number): string {
    if (level <= 0) {
      return 'menu-arrow';
    }

    return level === 1 ? 'menu-arrow inside-submenu' : 'menu-arrow inside-submenu inside-submenu-two';
  }

  onItemClick(event: MouseEvent, item: MenuItem, level: number): void {
    if (item.externalUrl) {
      return;
    }

    event.preventDefault();

    if (this.hasChildren(item)) {
      this.toggleItem(item, level);
      return;
    }

    if (item.path) {
      this.pageTabs.openTab({
        title: item.title === 'Admin Dashboard' ? 'Dashboard' : item.title,
        path: item.path,
        closable: item.closable ?? item.path !== '/dashboard',
      });

      this.closeMobileSidebar();
    }
  }

  private toggleItem(item: MenuItem, level: number): void {
    const opened = new Set<string>(this.openedIdsSignal());

    if (opened.has(item.id)) {
      opened.delete(item.id);
      this.removeChildrenIds(item, opened);
    } else {
      if (level === 0) {
        opened.clear();
      }
      opened.add(item.id);
    }

    this.openedIdsSignal.set(opened);
  }

  private removeChildrenIds(item: MenuItem, opened: Set<string>): void {
    for (const child of item.children ?? []) {
      opened.delete(child.id);
      this.removeChildrenIds(child, opened);
    }
  }

  private closeMobileSidebar(): void {
    document.documentElement.classList.remove('menu-opened');
    document.body.classList.remove('menu-opened');
    document.querySelector('.main-wrapper')?.classList.remove('slide-nav');
    document.querySelector('.sidebar-overlay')?.classList.remove('opened');
  }
}
