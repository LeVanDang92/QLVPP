export interface RoleMenuPermissionRow {
  menuId: string;
  menuName: string;
  menuGroup: string;
  parentMenuId: string | null;
  displayOrder: number;
  level: number;
  isSelected: boolean;
  canRead: boolean;
  canWrite: boolean;
  canDelete: boolean;
}

export interface UpdateRoleMenuPermissionItem {
  menuId: string;
  isSelected: boolean;
  canRead: boolean;
  canWrite: boolean;
  canDelete: boolean;
}

export interface UpdateRoleMenuPermissionsRequest {
  items: UpdateRoleMenuPermissionItem[];
}
