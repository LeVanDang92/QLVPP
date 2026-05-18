export interface MenuBadge {
  text: string;
  className?: string;
}

export interface MenuItem {
  id: string;
  title: string;
  icon?: string;
  path?: string;
  externalUrl?: string;
  closable?: boolean;
  badge?: MenuBadge;
  children?: MenuItem[];
}

export interface MenuSection {
  title: string;
  items: MenuItem[];
}
