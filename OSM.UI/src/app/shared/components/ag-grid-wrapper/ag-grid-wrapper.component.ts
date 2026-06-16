import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridOptions } from 'ag-grid-community';

@Component({
  selector: 'app-ag-grid',
  standalone: true,
  imports: [CommonModule, AgGridAngular],
  templateUrl: './ag-grid-wrapper.component.html',
  styleUrls: ['./ag-grid-wrapper.component.scss'],
})
export class AgGridWrapperComponent {
  @Input() title?: string;
  @Input() showSearch = true;
  @Input() rowData: any[] = [];
  @Input() columnDefs: ColDef[] = [];
  @Input() defaultColDef: ColDef = { sortable: true, filter: true, resizable: true };
  @Input() gridOptions: GridOptions = {};
  @Input() rowSelection: 'single' | 'multiple' = 'multiple';

  search = '';

  @Output() rowClicked = new EventEmitter<any>();
  @Output() selectionChanged = new EventEmitter<any>();

  onSearch(e: Event) {
    const val = (e.target as HTMLInputElement).value ?? '';
    this.search = val;
  }

  onRowClicked(e: any) {
    this.rowClicked.emit(e);
  }

  onSelectionChanged(e: any) {
    this.selectionChanged.emit(e);
  }
}
