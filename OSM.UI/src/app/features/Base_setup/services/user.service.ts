import { inject, Injectable } from "@angular/core";
import { User } from "../shared/models/User";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../../environments/environment";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class UserService {

  usersApiUrl =  `${environment.apiUrl}/${environment.apiVersion}/usersetting/users`;
  authRegisterUrl = `${environment.apiUrl}/auth/register`;
  private http = inject(HttpClient);

  getUsers() : Observable<User[]> {
    return this.http.get<User[]>(this.usersApiUrl);
  }

  getDepartment() : Observable<string[]> {
    return this.http.get<string[]>(`${environment.apiUrl}/${environment.apiVersion}/usersetting/departments`);
  }

  registerUser (user: User) : Observable<User> {

    user.fullName = user.userName;
    user.userName = user.userId;
    return this.http.post<User>(this.authRegisterUrl, user);
  }
}
