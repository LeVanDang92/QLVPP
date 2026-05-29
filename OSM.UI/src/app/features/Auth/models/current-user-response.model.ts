export type PermissionCode = string; // ['read', 'write', 'delete']
export type PermissionKey = string;  // ['dashboard.read', 'dashboard.write']

export interface MenuBadgeDto {
  text: string;
  className: string | null;
}

export interface MenuPermissionResponse {
  id: string; // menu id
  menuName: string;
  title: string | null; // short name
  menuType: string;
  menuGroup: string;
  path: string | null; // menu url
  externalUrl: string | null;
  icon: string | null;
  displayOrder: number;
  closable: boolean;
  parentMenuId: string | null;
  badge: MenuBadgeDto | null;
  children: MenuPermissionResponse[];
  permissions: PermissionCode[];
  permissionKeys: PermissionKey[];
}

export interface MenuSection {
  title: string;
  items: MenuPermissionResponse[];
}

export interface CurrentUserResponse {
  userId: string;
  userName: string;
  roles: string[];
  permissions: PermissionKey[];
  menus: MenuSection[];
}
