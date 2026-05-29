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


// MenuBadge   = nhãn nhỏ New/Hot
// MenuItem    = một menu hoặc submenu
// MenuSection = một nhóm menu
// children    = menu con
// path        = route Angular
// closable    = tab có cho đóng không
// externalUrl = link ngoài hệ thống
