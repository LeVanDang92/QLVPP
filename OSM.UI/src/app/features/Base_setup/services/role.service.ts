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

  getRoles() : Observable<Role[]> {
    return this.http.get<Role[]>(this.rolesApiUrl);
  }

}
