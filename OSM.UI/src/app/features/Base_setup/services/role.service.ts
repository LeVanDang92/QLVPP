import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role } from '../shared/models/Role';

@Injectable({
  providedIn: 'root',
})

export class RoleService {

  http = inject(HttpClient);
  rolesApiUrl =  `${environment.apiUrl}/${environment.apiVersion}/rolesetting/roles`;
  createRoleApiUrl = `${environment.apiUrl}/${environment.apiVersion}/rolesetting/roles`;
  updateRoleApiUrl = `${environment.apiUrl}/${environment.apiVersion}/rolesetting/roles`;
  deleteRoleApiUrl = `${environment.apiUrl}/${environment.apiVersion}/rolesetting/roles`;

  getRoles() : Observable<Role[]> {
    return this.http.get<Role[]>(this.rolesApiUrl);
  }

  createRole(role: Role): Observable<Role> {
    return this.http.post<Role>(this.createRoleApiUrl, role);
  }

  updateRole(role: Role): Observable<Role> {
    return this.http.put<Role>(`${this.updateRoleApiUrl}/${role.id}`, role);
  }

  deleteRole(roleId: string): Observable<void> {
    return this.http.delete<void>(`${this.deleteRoleApiUrl}/${roleId}`);
  }
}
