import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment.development";
import { Menu } from "../shared/models/Menu";
import { Observable } from "rxjs/internal/Observable";

@Injectable({
  providedIn: 'root'
})
export class MenuService {
  // This is a placeholder service. You can implement actual API calls here.

 http = inject(HttpClient);
 apiMenuUrl  =  `${environment.apiUrl}/${environment.apiVersion}/menus`;

  getMenus() : Observable<Menu[]> {
    return this.http.get<Menu[]>(this.apiMenuUrl);
  }

  createMenu(menu : Menu) : Observable<Menu> {
    return this.http.post<Menu>(this.apiMenuUrl, menu);
  }

  updateMenu(menuId: string, menuData: Menu) : Observable<Menu> {
    return this.http.put<Menu>(`${this.apiMenuUrl}/${menuId}`, menuData);
  }

  deleteMenu(menuId: string) : Observable<void> {
    return this.http.delete<void>(`${this.apiMenuUrl}/${menuId}`);
  }
}
