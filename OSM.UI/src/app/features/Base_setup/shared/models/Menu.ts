export interface Menu {
  menuId: string;
  menuName: string;
  menuShortName: string;
  menuType: string;
  menuGroup: string;
  menuUrl: string | null;
  externalUrl: string | null;
  iconClass: string;
  displayOrder: number;
  isActive: boolean;
  closable: boolean;
  badgeText: string | null;
  badgeClass: string | null;
  parentMenuId: string | null;
}
