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
  updateUserUrl = `${environment.apiUrl}/${environment.apiVersion}/usersetting/users`;
  deleteUserUrl = `${environment.apiUrl}/${environment.apiVersion}/usersetting/users`;

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

  updateUser (user: User) : Observable<User> {

    const requestUser = {
      userName: user.userId,
      fullName: user.userName ?? '',
      password: user.passwordShow ?? '',
      email: user.email,
      isActive: user.isActive,
      department: user.department,
      role: user.role ?? ''
    };

    return this.http.put<User>(`${this.updateUserUrl}/${user.userId}`, requestUser);
  }

  deleteUser (userId: string) : Observable<void> {
    return this.http.delete<void>(`${this.deleteUserUrl}/${userId}`);
  }
}
