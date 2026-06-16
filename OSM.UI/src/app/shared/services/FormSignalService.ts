import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FormSignalService {
  // Tạo một đường truyền tín hiệu bấm nút
  private saveTrigger = new Subject<'save'>();
  saveTrigger$ = this.saveTrigger.asObservable();

  // Hàm này sẽ được gọi khi bạn bấm nút Save
  triggerSave() {
    this.saveTrigger.next('save');
  }
}
