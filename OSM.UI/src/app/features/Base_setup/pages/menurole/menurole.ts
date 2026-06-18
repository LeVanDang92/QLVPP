import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  AllCommunityModule,
  CellValueChangedEvent,
  ColDef,
  GridOptions,
  ModuleRegistry,
  ValueFormatterParams,
} from 'ag-grid-community';
import { AgGridWrapperComponent } from '../../../../shared/components/ag-grid-wrapper/ag-grid-wrapper.component';
import { RoleService } from '../../services/role.service';
import { RoleMenuPermissionService } from '../../services/role-menu-permission.service';
import { SetupPageLayout } from '../../shared/layout/setup-page-layout/setup-page-layout';
import { Role } from '../../shared/models/Role';
import {
  RoleMenuPermissionRow,
  UpdateRoleMenuPermissionsRequest,
} from '../../shared/models/RoleMenuPermission';

ModuleRegistry.registerModules([AllCommunityModule]);

type PermissionField = 'isSelected' | 'canRead' | 'canWrite' | 'canDelete';

@Component({
  selector: 'app-menurole',
  imports: [CommonModule, SetupPageLayout, AgGridWrapperComponent],
  templateUrl: './menurole.html',
  styleUrl: './menurole.scss',
})
export class MenuroleComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private roleService = inject(RoleService);
  private roleMenuPermissionService = inject(RoleMenuPermissionService);

  breadcrumb = signal(this.route.snapshot.data['breadcrumb'] ?? '');
  title = signal(this.route.snapshot.data['title'] ?? '');
  sectionTitle = signal(this.route.snapshot.data['sectionTitle'] ?? '');

  roles = signal<Role[]>([]);
  selectedRoleId = signal<string>('');
  rowData = signal<RoleMenuPermissionRow[]>([]);
  originalRows = signal<RoleMenuPermissionRow[]>([]);
  hasChanged = signal(false);
  loading = signal(false);

  defaultColDef: ColDef<RoleMenuPermissionRow> = {
    sortable: true,
    filter: true,
    resizable: true,
    minWidth: 100,
  };

  gridOptions: GridOptions<RoleMenuPermissionRow> = {
    theme: 'legacy',
    suppressRowClickSelection: true,
    rowHeight: 36,
  };

  colDefs: ColDef<RoleMenuPermissionRow>[] = [
    {
      field: 'menuName',
      headerName: 'Menu',
      minWidth: 320,
      flex: 1,
      cellRenderer: (params: any) => this.menuCellRenderer(params),
    },
    {
      field: 'menuGroup',
      headerName: 'Group',
      width: 140,
    },
    {
      field: 'parentMenuId',
      headerName: 'Parent',
      width: 140,
      valueFormatter: (params: ValueFormatterParams<RoleMenuPermissionRow, string | null>) =>
        params.value ?? '',
    },
    {
      field: 'isSelected',
      headerName: 'Use',
      width: 90,
      cellRenderer: (params: any) => this.checkboxRenderer(params, 'isSelected'),
    },
    {
      field: 'canRead',
      headerName: 'Read',
      width: 90,
      cellRenderer: (params: any) => this.checkboxRenderer(params, 'canRead'),
    },
    {
      field: 'canWrite',
      headerName: 'Write',
      width: 90,
      cellRenderer: (params: any) => this.checkboxRenderer(params, 'canWrite'),
    },
    {
      field: 'canDelete',
      headerName: 'Delete',
      width: 90,
      cellRenderer: (params: any) => this.checkboxRenderer(params, 'canDelete'),
    },
  ];

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.roleService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: (error) => {
        console.error('Failed to load roles:', error);
        alert('Failed to load roles. Please try again later.');
      },
    });
  }

  onRoleChange(event: Event): void {
    const roleId = (event.target as HTMLSelectElement).value;
    this.selectedRoleId.set(roleId);

    if (!roleId) {
      this.setRows([]);
      return;
    }

    this.loadRoleMenuPermissions(roleId);
  }

  onUpdate(): void {
    const roleId = this.selectedRoleId();

    if (!roleId) {
      alert('Please select role.');
      return;
    }

    if (!this.hasChanged()) {
      alert('No changes to update.');
      return;
    }

    const selectedRole = this.roles().find((role) => role.id === roleId);

    const confirmed = confirm(
      `Are you sure you want to update menu permissions for role "${selectedRole?.name ?? roleId}"?`,
    );

    if (!confirmed) {
      return;
    }

    const request: UpdateRoleMenuPermissionsRequest = {
      items: this.rowData().map((row) => ({
        menuId: row.menuId,
        isSelected: row.isSelected,
        canRead: row.canRead,
        canWrite: row.canWrite,
        canDelete: row.canDelete,
      })),
    };

    this.roleMenuPermissionService.updateByRole(roleId, request).subscribe({
      next: (rows) => {
        this.setRows(rows);
        alert('Update role menu permission success!');
      },
      error: (error) => {
        console.error('Update role menu permission failed:', error);
        alert('Update role menu permission failed. Please try again later.');
      },
    });
  }

  onReset(): void {
    this.rowData.set(this.cloneRows(this.originalRows()));
    this.hasChanged.set(false);
  }

  onCellValueChanged(event: CellValueChangedEvent<RoleMenuPermissionRow>): void {
    if (!event.data || !event.colDef.field) {
      return;
    }

    const field = event.colDef.field as PermissionField;

    if (!this.isPermissionField(field)) {
      return;
    }

    const changedMenuId = event.data.menuId;
    const newValue = Boolean(event.newValue);
    const rows = this.cloneRows(this.rowData());
    const changedRow = rows.find((row) => row.menuId === changedMenuId);

    if (!changedRow) {
      return;
    }

    changedRow[field] = newValue;
    this.applyPermissionRules(rows, changedRow, field);

    this.rowData.set(rows);
    this.updateChangedState(rows);
  }

  private loadRoleMenuPermissions(roleId: string): void {
    this.loading.set(true);

    this.roleMenuPermissionService.getByRole(roleId).subscribe({
      next: (rows) => {
        this.setRows(rows);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load role menu permissions:', error);
        this.setRows([]);
        this.loading.set(false);
        alert('Failed to load role menu permissions. Please try again later.');
      },
    });
  }

  private setRows(rows: RoleMenuPermissionRow[]): void {
    const normalizedRows = this.cloneRows(rows);
    this.rowData.set(normalizedRows);
    this.originalRows.set(this.cloneRows(normalizedRows));
    this.hasChanged.set(false);
  }

  private applyPermissionRules(
    rows: RoleMenuPermissionRow[],
    changedRow: RoleMenuPermissionRow,
    changedField: PermissionField,
  ): void {
    if (changedField === 'isSelected') {
      if (changedRow.isSelected) {
        changedRow.canRead = true;
      } else {
        changedRow.canRead = false;
        changedRow.canWrite = false;
        changedRow.canDelete = false;
      }
    }

    if (changedField === 'canRead') {
      if (changedRow.canRead) {
        changedRow.isSelected = true;
      } else {
        changedRow.isSelected = false;
        changedRow.canWrite = false;
        changedRow.canDelete = false;
      }
    }

    if (changedField === 'canWrite') {
      if (changedRow.canWrite) {
        changedRow.isSelected = true;
        changedRow.canRead = true;
      }
    }

    if (changedField === 'canDelete') {
      if (changedRow.canDelete) {
        changedRow.isSelected = true;
        changedRow.canRead = true;
      }
    }

    this.syncParentMenus(rows);
  }

  private syncParentMenus(rows: RoleMenuPermissionRow[]): void {
    const rowById = new Map(rows.map((row) => [row.menuId, row]));

    const childrenByParent = new Map<string, RoleMenuPermissionRow[]>();

    rows.forEach((row) => {
      if (!row.parentMenuId) {
        return;
      }

      const children = childrenByParent.get(row.parentMenuId) ?? [];
      children.push(row);
      childrenByParent.set(row.parentMenuId, children);
    });

    const orderedRows = [...rows].sort((a, b) => (b.level ?? 0) - (a.level ?? 0));

    orderedRows.forEach((row) => {
      const children = childrenByParent.get(row.menuId) ?? [];

      if (children.length === 0) {
        return;
      }

      const hasSelectedChild = children.some((child) => this.isRowSelected(child));

      if (hasSelectedChild) {
        row.isSelected = true;
        row.canRead = true;
        return;
      }

      row.isSelected = false;
      row.canRead = false;
      row.canWrite = false;
      row.canDelete = false;
    });

    rows.forEach((row) => {
      if (!this.isRowSelected(row)) {
        return;
      }

      let parentMenuId = row.parentMenuId;
      const visited = new Set<string>();

      while (parentMenuId && !visited.has(parentMenuId)) {
        visited.add(parentMenuId);

        const parentRow = rowById.get(parentMenuId);

        if (!parentRow) {
          return;
        }

        parentRow.isSelected = true;
        parentRow.canRead = true;

        parentMenuId = parentRow.parentMenuId;
      }
    });
  }

  private isRowSelected(row: RoleMenuPermissionRow): boolean {
    return Boolean(row.isSelected || row.canRead || row.canWrite || row.canDelete);
  }

  private updateChangedState(rows: RoleMenuPermissionRow[]): void {
    this.hasChanged.set(
      this.toPermissionSnapshot(rows) !== this.toPermissionSnapshot(this.originalRows()),
    );
  }

  private toPermissionSnapshot(rows: RoleMenuPermissionRow[]): string {
    return JSON.stringify(
      rows.map((row) => ({
        menuId: row.menuId,
        isSelected: row.isSelected,
        canRead: row.canRead,
        canWrite: row.canWrite,
        canDelete: row.canDelete,
      })),
    );
  }

  private checkboxRenderer(params: any, field: PermissionField): HTMLInputElement {
    const input = document.createElement('input');
    input.type = 'checkbox';
    input.checked = Boolean(params.value);
    input.className = 'permission-checkbox';

    input.addEventListener('change', (event) => {
      const checked = (event.target as HTMLInputElement).checked;
      params.node.setDataValue(field, checked);
    });

    return input;
  }

  private menuCellRenderer(params: any): HTMLSpanElement {
    const row = params.data as RoleMenuPermissionRow;
    const wrapper = document.createElement('span');
    wrapper.className = 'menu-tree-cell';
    wrapper.style.paddingLeft = `${(row?.level ?? 0) * 22}px`;

    const prefix = document.createElement('span');
    prefix.className = 'menu-tree-prefix';
    prefix.textContent = row?.level ? '↳' : '';

    const text = document.createElement('span');
    text.textContent = row?.menuName ?? '';

    wrapper.appendChild(prefix);
    wrapper.appendChild(text);

    return wrapper;
  }

  private isPermissionField(field: string): field is PermissionField {
    return (
      field === 'isSelected' || field === 'canRead' || field === 'canWrite' || field === 'canDelete'
    );
  }

  private cloneRows(rows: RoleMenuPermissionRow[]): RoleMenuPermissionRow[] {
    return rows.map((row) => ({ ...row }));
  }
}
