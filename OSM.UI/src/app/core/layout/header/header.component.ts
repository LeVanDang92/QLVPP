import { AfterViewInit, Component, HostListener } from '@angular/core';
import { HorizontalMenuComponent } from '../menu/horizontal-menu/horizontal-menu.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [HorizontalMenuComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements AfterViewInit {
  ngAfterViewInit(): void {
    this.hideGlobalLoader();
  }

  @HostListener('click', ['$event'])
  handleClick(event: MouseEvent): void {
    const target = event.target as HTMLElement | null;

    if (!target) {
      return;
    }

    if (target.closest('#mobile_btn')) {
      event.preventDefault();
      this.toggleMobileSidebar();
      return;
    }

    if (target.closest('.btnFullscreen')) {
      event.preventDefault();
      this.toggleFullscreen();
    }
  }

  private toggleMobileSidebar(): void {
    document.documentElement.classList.toggle('menu-opened');
    document.body.classList.toggle('menu-opened');
    document.querySelector('.main-wrapper')?.classList.toggle('slide-nav');
    document.querySelector('.sidebar-overlay')?.classList.toggle('opened');
  }

  private toggleFullscreen(): void {
    if (!document.fullscreenElement) {
      document.documentElement.requestFullscreen?.();
      return;
    }

    document.exitFullscreen?.();
  }

  private hideGlobalLoader(): void {
    window.setTimeout(() => {
      document.getElementById('global-loader')?.remove();
    }, 150);
  }
}
