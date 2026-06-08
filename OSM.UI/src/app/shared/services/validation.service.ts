import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FormValidationErrorService {
  private errorMessages: Record<string, (args: any) => string> = {
    required: () => 'Trường này không được để trống.',
    email: () => 'Định dạng email không hợp lệ.',
    minlength: (args) => `Phải nhập tối thiểu ${args.requiredLength} ký tự.`,
    maxlength: (args) => `Chỉ được nhập tối đa ${args.requiredLength} ký tự.`,
    pattern: () => 'Dữ liệu nhập vào không đúng định dạng.'
  };

  getErrorMessage(errorKey: string, errorValue: any): string {
    const messageFn = this.errorMessages[errorKey];
    return messageFn ? messageFn(errorValue) : `Lỗi không xác định (${errorKey})`;
  }
}
