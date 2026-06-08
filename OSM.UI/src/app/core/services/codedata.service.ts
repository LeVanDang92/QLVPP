import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CodeDataDto } from '../models/codeDataDto';

@Injectable({
  providedIn: 'root',
})
export class CodedataService {

  http = inject(HttpClient);
  codedataApiUrl =  `${environment.apiUrl}/${environment.apiVersion}/codedata`;

  getCodeData(tableCode : string) : Observable<CodeDataDto[]> {
    return this.http.get<CodeDataDto[]>(`${this.codedataApiUrl}/${tableCode}`);
  }

}
