import { Injectable, effect, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { DASHBOARD_TAB_PATH, titleFromPath } from '../menu/menu.data';

export interface PageTab {
  title: string;
  path: string;
  closable: boolean;
}

interface PageTabStorageState {
  tabs: PageTab[];
  activePath: string;
}

@Injectable({
  providedIn: 'root',
})
export class PageTabService {
  private readonly router = inject(Router);
  private readonly storageKey = 'osm.page-tabs.v1';
  private readonly defaultTab: PageTab = {
    title: 'Dashboard',
    path: DASHBOARD_TAB_PATH,
    closable: false,
  };

  private readonly restoredState = this.loadState();
  private readonly tabsSignal = signal<PageTab[]>(this.restoredState.tabs);
  private readonly activePathSignal = signal<string>(this.restoredState.activePath);

  readonly tabs = this.tabsSignal.asReadonly();
  readonly activePath = this.activePathSignal.asReadonly();

  constructor() {
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd)).subscribe((event) => {
      const path = this.normalizePath(event.urlAfterRedirects.split('?')[0] || DASHBOARD_TAB_PATH);
      this.ensureTabExists(path);
      this.activePathSignal.set(path);
    });

    effect(() => {
      this.saveState({
        tabs: this.tabsSignal(),
        activePath: this.activePathSignal(),
      });
    });
  }

  openTab(tab: PageTab): void {
    const normalizedTab: PageTab = {
      ...tab,
      path: this.normalizePath(tab.path),
    };

    const exists = this.tabsSignal().some((item) => item.path === normalizedTab.path);

    if (!exists) {
      this.tabsSignal.update((tabs) => [...tabs, normalizedTab]);
    }

    this.activePathSignal.set(normalizedTab.path);
    this.router.navigateByUrl(normalizedTab.path);
  }

  activateTab(path: string): void {
    const normalizedPath = this.normalizePath(path);
    this.activePathSignal.set(normalizedPath);
    this.router.navigateByUrl(normalizedPath);
  }

  closeTab(path: string): void {
    const normalizedPath = this.normalizePath(path);
    const tabs = this.tabsSignal();
    const tabIndex = tabs.findIndex((item) => item.path === normalizedPath);
    const tab = tabs[tabIndex];

    if (!tab || !tab.closable) {
      return;
    }

    const newTabs = tabs.filter((item) => item.path !== normalizedPath);
    this.tabsSignal.set(newTabs.length ? newTabs : [this.defaultTab]);

    if (this.activePathSignal() === normalizedPath) {
      const nextTab = newTabs[tabIndex - 1] ?? newTabs[0] ?? this.defaultTab;
      this.activateTab(nextTab.path);
    }
  }

  closeOtherTabs(path: string): void {
    const normalizedPath = this.normalizePath(path);
    const currentTab = this.tabsSignal().find((tab) => tab.path === normalizedPath);

    this.tabsSignal.set([this.defaultTab, ...(currentTab && currentTab.path !== DASHBOARD_TAB_PATH ? [currentTab] : [])]);
    this.activateTab(currentTab?.path ?? DASHBOARD_TAB_PATH);
  }

  closeAllClosableTabs(): void {
    this.tabsSignal.set([this.defaultTab]);
    this.activateTab(DASHBOARD_TAB_PATH);
  }

  clearSavedTabs(): void {
    this.removeState();
    this.tabsSignal.set([this.defaultTab]);
    this.activateTab(DASHBOARD_TAB_PATH);
  }

  private ensureTabExists(path: string): void {
    const normalizedPath = this.normalizePath(path);

    if (this.tabsSignal().some((tab) => tab.path === normalizedPath)) {
      return;
    }

    this.tabsSignal.update((tabs) => [
      ...tabs,
      {
        title: titleFromPath(normalizedPath),
        path: normalizedPath,
        closable: normalizedPath !== DASHBOARD_TAB_PATH,
      },
    ]);
  }

  private normalizePath(path: string): string {
    if (!path || path === '/') {
      return DASHBOARD_TAB_PATH;
    }

    return path.startsWith('/') ? path : `/${path}`;
  }

  private loadState(): PageTabStorageState {
    const fallback: PageTabStorageState = {
      tabs: [this.defaultTab],
      activePath: DASHBOARD_TAB_PATH,
    };

    if (!this.canUseStorage()) {
      return fallback;
    }

    try {
      const raw = localStorage.getItem(this.storageKey);

      if (!raw) {
        return fallback;
      }

      const parsed = JSON.parse(raw) as Partial<PageTabStorageState>;
      const tabs = this.normalizeTabs(parsed.tabs);
      const activePath = this.normalizePath(parsed.activePath ?? tabs[0]?.path ?? DASHBOARD_TAB_PATH);

      return {
        tabs,
        activePath: tabs.some((tab) => tab.path === activePath) ? activePath : DASHBOARD_TAB_PATH,
      };
    } catch {
      return fallback;
    }
  }

  private normalizeTabs(value: unknown): PageTab[] {
    if (!Array.isArray(value)) {
      return [this.defaultTab];
    }

    const tabs = value
      .filter((item): item is PageTab => {
        return (
          typeof item === 'object' &&
          item !== null &&
          'title' in item &&
          'path' in item &&
          typeof (item as { title?: unknown }).title === 'string' &&
          typeof (item as { path?: unknown }).path === 'string'
        );
      })
      .map((item) => ({
        title: item.path === DASHBOARD_TAB_PATH ? 'Dashboard' : item.title,
        path: this.normalizePath(item.path),
        closable: item.path !== DASHBOARD_TAB_PATH && item.closable !== false,
      }));

    const uniqueTabs = new Map<string, PageTab>();
    uniqueTabs.set(DASHBOARD_TAB_PATH, this.defaultTab);

    for (const tab of tabs) {
      uniqueTabs.set(tab.path, tab);
    }

    return Array.from(uniqueTabs.values());
  }

  private saveState(state: PageTabStorageState): void {
    if (!this.canUseStorage()) {
      return;
    }

    localStorage.setItem(this.storageKey, JSON.stringify(state));
  }

  private removeState(): void {
    if (!this.canUseStorage()) {
      return;
    }

    localStorage.removeItem(this.storageKey);
  }

  private canUseStorage(): boolean {
    return typeof window !== 'undefined' && typeof window.localStorage !== 'undefined';
  }
}
