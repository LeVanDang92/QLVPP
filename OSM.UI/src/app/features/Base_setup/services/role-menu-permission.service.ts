import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  RoleMenuPermissionRow,
  UpdateRoleMenuPermissionsRequest,
} from '../shared/models/RoleMenuPermission';

@Injectable({
  providedIn: 'root',
})
export class RoleMenuPermissionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/${environment.apiVersion}/rolemenupermissions`;

  getByRole(roleId: string): Observable<RoleMenuPermissionRow[]> {
    return this.http.get<RoleMenuPermissionRow[]>(`${this.apiUrl}/${roleId}`);
  }

  updateByRole(
    roleId: string,
    request: UpdateRoleMenuPermissionsRequest
  ): Observable<RoleMenuPermissionRow[]> {
    return this.http.put<RoleMenuPermissionRow[]>(`${this.apiUrl}/${roleId}`, request);
  }
}
