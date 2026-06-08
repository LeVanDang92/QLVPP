import { Directive, inject, ComponentRef, ViewContainerRef, DestroyRef, OnInit, ChangeDetectorRef, Optional } from '@angular/core';
import { NgControl, FormGroupDirective } from '@angular/forms'; // Lắng nghe form chuẩn
import { FormValidationErrorService } from '../services/validation.service';
import { FormErrorComponent } from '../components/form-error';
import { FormSignalService } from  '../services/FormSignalService'; // Lắng nghe nút bấm ngoài
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { merge, startWith, Observable } from 'rxjs';

@Directive({
  selector: '[formControlName], [formControl]'
})
export class FormErrorDirective implements OnInit {
  private ngControl = inject(NgControl);
  private vcr = inject(ViewContainerRef);
  private errorService = inject(FormValidationErrorService);
  private destroyRef = inject(DestroyRef);
  private cdr = inject(ChangeDetectorRef);

  // Inject cả 2 nguồn kích hoạt
  private formSignalService = inject(FormSignalService);
  private formGroupDirective = inject(FormGroupDirective, { optional: true });

  private componentRef: ComponentRef<FormErrorComponent> | null = null;

  ngOnInit() {
    const control = this.ngControl.control;
    if (control) {
      // 1. Mặc định luôn lắng nghe thay đổi trạng thái và giá trị của chính ô input đó
      const triggers: Observable<any>[] = [
        control.statusChanges,
        control.valueChanges,
        this.formSignalService.saveTrigger$ // Luôn lắng nghe tín hiệu từ nút bấm ngoài
      ];

      // 2. Kịch bản dùng Submit: Nếu ô input này nằm trong một thẻ <form>, lắng nghe thêm ngSubmit
      if (this.formGroupDirective) {
        triggers.push(this.formGroupDirective.ngSubmit);
      }

      // 3. Hợp nhất tất cả các nguồn lại thành một luồng xử lý duy nhất
      merge(...triggers)
        .pipe(
          startWith(null),
          takeUntilDestroyed(this.destroyRef)
        )
        .subscribe(() => {
          this.updateErrorMessage();
        });
    }
  }

  private updateErrorMessage() {
    const control = this.ngControl.control;

    // Kiểm tra xem form HTML đã được submit chưa (dành cho kịch bản nút submit)
    const isFormSubmitted = this.formGroupDirective?.submitted;

    // Điều kiện hiện lỗi: Dữ liệu sai VÀ (Đã bị chạm HOẶC Form đã submit)
    const shouldShowError = control && control.invalid && (control.touched || isFormSubmitted);

    if (shouldShowError) {
      const firstErrorKey = Object.keys(control.errors || {})[0];
      const errorValue = control.errors?.[firstErrorKey];
      const errorText = this.errorService.getErrorMessage(firstErrorKey, errorValue);

      if (!this.componentRef) {
        this.componentRef = this.vcr.createComponent(FormErrorComponent);
      }
      this.componentRef.setInput('message', errorText);
      this.componentRef.changeDetectorRef.detectChanges();
    } else {
      if (this.componentRef) {
        this.componentRef.destroy();
        this.componentRef = null;
      }
    }
    this.cdr.markForCheck();
  }
}
