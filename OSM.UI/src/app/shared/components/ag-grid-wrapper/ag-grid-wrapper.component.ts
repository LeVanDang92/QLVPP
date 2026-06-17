import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridAngular } from 'ag-grid-angular';
import {
  CellValueChangedEvent,
  ColDef,
  GetRowIdParams,
  GridApi,
  GridOptions,
  GridReadyEvent,
} from 'ag-grid-community';

@Component({
  selector: 'app-ag-grid',
  standalone: true,
  imports: [CommonModule, AgGridAngular],
  templateUrl: './ag-grid-wrapper.component.html',
  styleUrls: ['./ag-grid-wrapper.component.scss'],
})
export class AgGridWrapperComponent implements OnChanges {
  @Input() title?: string;
  @Input() showSearch = true;

  @Input() rowData: any[] = [];
  @Input() columnDefs: ColDef[] = [];
  @Input() defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
  };

  @Input() gridOptions: GridOptions = {};
  @Input() rowSelection: 'single' | 'multiple' = 'multiple';

  /**
   * Bật chế độ batch edit.
   * Lưu ý: column nào được edit vẫn phải set editable: true trong columnDefs.
   */
  @Input() enableBatchEdit = false;

  /**
   * Field dùng làm khóa chính của row.
   * Ví dụ Menu dùng menuId, Product dùng productId.
   */
  @Input() rowIdField = '';

  /**
   * Có hiển thị nút Save Batch trong wrapper hay không.
   */
  @Input() showBatchActions = false;

  @Output() rowClicked = new EventEmitter<any>();
  @Output() selectionChanged = new EventEmitter<any>();

  /**
   * Emit mỗi khi danh sách row đã sửa thay đổi.
   */
  @Output() editedRowsChanged = new EventEmitter<any[]>();

  /**
   * Emit khi bấm Save Batch trong wrapper.
   */
  @Output() batchSave = new EventEmitter<any[]>();

  @Output() cellValueChanged = new EventEmitter<CellValueChangedEvent<any>>();

  search = '';
  editedRowsCount = 0;

  private gridApi?: GridApi;

  private originalRows = new Map<string, string>();
  private editedRows = new Map<string, any>();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['rowData']) {
      this.resetBatchTracking();
    }
  }

  getRowId = (params: GetRowIdParams<any>): string => {
    return this.getRowKey(params.data);
  };

  onGridReady(event: GridReadyEvent): void {
    this.gridApi = event.api;
  }

  onSearch(e: Event): void {
    const val = (e.target as HTMLInputElement).value ?? '';
    this.search = val;
  }

  onRowClicked(e: any): void {
    this.rowClicked.emit(e);
  }

  onSelectionChanged(e: any): void {
    this.selectionChanged.emit(e);
  }

onCellValueChanged(event: CellValueChangedEvent<any>): void {
  this.cellValueChanged.emit(event);

  if (!this.enableBatchEdit) {
    return;
  }

  const row = event.data;

  if (!row) {
    return;
  }

  const rowKey = this.getRowKey(row);
  const originalSnapshot = this.originalRows.get(rowKey);
  const currentSnapshot = this.toStableJson(row);

  if (!originalSnapshot) {
    this.editedRows.set(rowKey, this.clone(row));
  } else if (originalSnapshot !== currentSnapshot) {
    this.editedRows.set(rowKey, this.clone(row));
  } else {
    this.editedRows.delete(rowKey);
  }

  this.emitEditedRows();
}

  getEditedRows(): any[] {
    return Array.from(this.editedRows.values()).map((row) => this.clone(row));
  }

  clearEditedRows(): void {
    this.resetBatchTracking();
  }

  onBatchSave(): void {
    this.batchSave.emit(this.getEditedRows());
  }

  private resetBatchTracking(): void {
    this.originalRows.clear();
    this.editedRows.clear();

    for (const row of this.rowData ?? []) {
      const rowKey = this.getRowKey(row);
      this.originalRows.set(rowKey, this.toStableJson(row));
    }

    this.emitEditedRows();
  }

  private emitEditedRows(): void {
    const rows = this.getEditedRows();
    this.editedRowsCount = rows.length;
    this.editedRowsChanged.emit(rows);
  }

  private getRowKey(row: any): string {
    if (this.rowIdField && row?.[this.rowIdField] !== undefined && row?.[this.rowIdField] !== null) {
      return String(row[this.rowIdField]);
    }

    const index = this.rowData.indexOf(row);
    return `row-${index}`;
  }

  private toStableJson(value: any): string {
    return JSON.stringify(this.sortObject(value));
  }

  private sortObject(value: any): any {
    if (Array.isArray(value)) {
      return value.map((item) => this.sortObject(item));
    }

    if (value !== null && typeof value === 'object') {
      return Object.keys(value)
        .sort()
        .reduce((result: any, key) => {
          result[key] = this.sortObject(value[key]);
          return result;
        }, {});
    }

    return value;
  }

  private clone<T>(value: T): T {
    return typeof structuredClone === 'function'
      ? structuredClone(value)
      : JSON.parse(JSON.stringify(value));
  }
}
