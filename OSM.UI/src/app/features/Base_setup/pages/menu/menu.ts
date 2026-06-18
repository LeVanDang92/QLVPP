import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Menu } from '../../shared/models/Menu';
import { SetupPageLayout } from '../../shared/layout/setup-page-layout/setup-page-layout';
import { ModuleRegistry, AllCommunityModule, ColDef, GridOptions } from 'ag-grid-community';
import { AgGridWrapperComponent } from '../../../../shared/components/ag-grid-wrapper/ag-grid-wrapper.component';
import { MenuService } from '../../services/menu.service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormErrorDirective } from '../../../../shared/Directive/form-error.directive';
import { CodedataService } from '../../../../core/services/codedata.service';
import { FormSignalService } from '../../../../shared/services/FormSignalService';
import { HttpErrorResponse } from '@angular/common/http';
import { ServerValidationErrorService } from '../../../../shared/services/server-validation-error.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

ModuleRegistry.registerModules([AllCommunityModule]);

@Component({
  selector: 'app-menu',
  imports: [SetupPageLayout, AgGridWrapperComponent, ReactiveFormsModule, FormErrorDirective,TranslatePipe],
  templateUrl: './menu.html',
  styleUrls: ['./menu.scss'],
})
export class MenuComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private menuService = inject(MenuService);
  private codeDataService = inject(CodedataService);
  private formSignalService = inject(FormSignalService);
  private serverValidationErrorService = inject(ServerValidationErrorService);
  private readonly translate = inject(TranslateService);

  breadcrumb = signal(this.route.snapshot.data['breadcrumb'] ?? '');
  title = signal(this.route.snapshot.data['title'] ?? '');
  sectionTitle = signal(this.route.snapshot.data['sectionTitle'] ?? '');

  selectedMenu = signal<Menu | null>(null);
  searchTerm = signal<string>('');

  defaultColDef: ColDef = { sortable: true, filter: true, resizable: true, minWidth: 100 };
  rowSelection: 'single' | 'multiple' = 'multiple';
  rowMultiSelectWithClick = true;
  suppressRowClickSelection = false;

  gridOptions: GridOptions = { theme: 'legacy' };

  rowData = signal<Menu[]>([]);

  colDefs: ColDef[] = [
    {
      headerName: '',
      checkboxSelection: true,
      headerCheckboxSelection: true,
      width: 20,
      pinned: 'left',
      editable: false,
    },
    { field: 'menuId', headerName: 'Menu ID', editable: false },
    { field: 'menuName', headerName: 'Menu Name', editable: false },
    { field: 'menuShortName', headerName: 'Menu Short Name', editable: false },
    { field: 'menuType', headerName: 'Menu Type', editable: false },
    { field: 'menuGroup', headerName: 'Menu Group', editable: false },
    { field: 'menuUrl', headerName: 'Menu URL', editable: false },
    { field: 'externalUrl', headerName: 'External URL', editable: false },
    { field: 'parentMenuId', headerName: 'Parent Menu ID', editable: false },
    { field: 'iconClass', headerName: 'Icon Class', editable: false },
    { field: 'displayOrder', headerName: 'Display Order', editable: false },
    { field: 'isActive', headerName: 'Is Active', editable: false },
    { field: 'closable', headerName: 'Closable', editable: false },
    { field: 'badgeText', headerName: 'Badge Text', editable: false },
    { field: 'badgeClass', headerName: 'Badge Class', editable: false },
  ];

  private fb = inject(FormBuilder);

  form = this.fb.group({
    menuId: ['', [Validators.required, Validators.maxLength(50)]],
    menuName: ['', [Validators.required, Validators.maxLength(200)]],
    menuShortName: ['', [Validators.required, Validators.maxLength(100)]],
    menuType: ['', [Validators.required, Validators.maxLength(50)]],
    menuGroup: ['', [Validators.required, Validators.maxLength(100)]],
    menuUrl: ['', [Validators.maxLength(500)]],
    externalUrl: ['', [Validators.maxLength(1000)]],
    parentMenuId: ['', [Validators.maxLength(50)]],
    iconClass: ['', [Validators.maxLength(100)]],
    displayOrder: [0],
    isActive: [true],
    closable: [true],
    badgeText: ['', Validators.maxLength(50)],
    badgeClass: ['', Validators.maxLength(200)],
  });

  menuTypes = signal<{ data_Code: string; data_Value: string }[]>([]);
  menuGroups = signal<{ data_Code: string; data_Value: string }[]>([]);

  constructor() {
    effect(() => {
      const menuIdControl = this.form.get('menuId');

      if (this.selectedMenu()) {
        menuIdControl?.disable();
      } else {
        menuIdControl?.enable();
      }
    });
  }

  ngOnInit() {
    this.loadMenus();
    this.loadMenuTypes();
    this.loadMenuGroups();
  }

  loadMenus() {
    this.menuService.getMenus().subscribe((menus: Menu[]) => {
      this.rowData.set(menus);
    });
  }

  loadMenuTypes() {
    this.codeDataService
      .getCodeData('MENU_TYPE')
      .subscribe((types: { data_Code: string; data_Value: string }[]) => {
        this.menuTypes.set(types);
      });
  }

  loadMenuGroups() {
    this.codeDataService
      .getCodeData('MENU_GROUP')
      .subscribe((groups: { data_Code: string; data_Value: string }[]) => {
        this.menuGroups.set(groups);
      });
  }

  onRowClicked(event: any) {
    this.selectedMenu.set(event.data ?? null);
    this.form.patchValue(event.data ?? {});
  }

  onCreate() {
    this.onSave();
  }

  onUpdate() {
    this.onSave();
  }

  onDelete() {
    const menuId = this.selectedMenu()?.menuId;

    if (!menuId) {
      alert('Please select menu to delete.');
      return;
    }

    if (!confirm(`Are you sure you want to delete menu "${menuId}"?`)) {
      return;
    }

    this.menuService.deleteMenu(menuId).subscribe({
      next: () => {
        alert('Delete menu success!');
        this.clearSelectionAndForm();
      },
      error: (error: HttpErrorResponse) => {
        this.serverValidationErrorService.applyErrors(this.form, error);
        alert('Delete menu error!');
      },
    });
  }

  onReset() {
    this.clearSelectionAndForm();
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.formSignalService.triggerSave();
      return;
    }

    const formValue = this.form.getRawValue();

    const menuToSave: Menu = {
      ...this.selectedMenu(),
      ...formValue,
      menuId: formValue.menuId ?? '',
      menuName: formValue.menuName ?? '',
      menuShortName: formValue.menuShortName ?? '',
      menuType: formValue.menuType ?? '',
      menuGroup: formValue.menuGroup ?? '',
      menuUrl: formValue.menuUrl ?? '',
      externalUrl: formValue.externalUrl ?? '',
      parentMenuId: formValue.parentMenuId ?? '',
      iconClass: formValue.iconClass ?? '',
      badgeClass: formValue.badgeClass ?? '',
      badgeText: formValue.badgeText ?? '',
      displayOrder: Number(formValue.displayOrder),
      isActive: Boolean(formValue.isActive),
      closable: Boolean(formValue.closable),
    };

    const selectedId = this.selectedMenu()?.menuId;

    if (!selectedId) {
      this.createMenu(menuToSave);
    } else {
      this.updateMenu(menuToSave);
    }
  }

  private updateMenu(menu: Menu): void {
    if (!confirm(this.translate.instant('COMMON.CONFIRM_UPDATE'))) {
      return;
    }

    this.menuService.updateMenu(menu.menuId, menu).subscribe({
      next: (updatedMenu) => {
        this.rowData.update((menus) =>
          menus.map((item) => (item.menuId === updatedMenu.menuId ? updatedMenu : item)),
        );

        this.clearSelectionAndForm();
        alert('Update menu success!');
      },
      error: (error: HttpErrorResponse) => {
        this.serverValidationErrorService.applyErrors(this.form, error);
        alert('Update menu error.Please try again later.');
      },
    });
  }

  private createMenu(menu: Menu): void {
    this.menuService.createMenu(menu).subscribe({
      next: (createdMenu) => {
        this.rowData.update((menus) => [...menus, createdMenu]);

        this.clearSelectionAndForm();
        alert('Create menu sucess!');
      },
      error: (err: HttpErrorResponse) => {
        this.serverValidationErrorService.applyErrors(this.form, err);
        alert('create menu error. Please try again.');
      },
    });
  }

  private clearSelectionAndForm(): void {
    this.selectedMenu.set(null);
    this.form.reset({ isActive: true, closable: true });
    this.serverValidationErrorService.clearServerErrors(this.form);
  }
}
