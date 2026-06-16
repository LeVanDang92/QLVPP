import { Injectable } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ApiProblemDetails } from '../../core/models/api-problem-details.model';

@Injectable({
  providedIn: 'root',
})
export class ServerValidationErrorService {
  applyErrors(
    form: FormGroup,
    error: HttpErrorResponse,
    fieldMap?: Record<string, string>
  ): boolean {
    const apiError = error.error as ApiProblemDetails;

    if (!apiError?.errors) {
      return false;
    }

    Object.entries(apiError.errors).forEach(([field, messages]) => {
      const controlName = fieldMap?.[field] ?? field;
      const control = form.get(controlName);

      if (!control) {
        return;
      }

      this.setServerError(control, messages?.[0]);
    });

    return true;
  }

  clearServerErrors(form: FormGroup): void {
    Object.values(form.controls).forEach(control => {
      this.clearServerError(control);

      if (control instanceof FormGroup) {
        this.clearServerErrors(control);
      }
    });
  }

  private setServerError(control: AbstractControl, message?: string): void {
    const currentErrors = control.errors ?? {};

    control.setErrors({
      ...currentErrors,
      serverError: message || 'Dữ liệu không hợp lệ.',
    });

    control.markAsTouched();
    control.markAsDirty();
  }

  private clearServerError(control: AbstractControl): void {
    if (!control.errors?.['serverError']) {
      return;
    }

    const { serverError, ...remainingErrors } = control.errors;

    control.setErrors(
      Object.keys(remainingErrors).length > 0 ? remainingErrors : null
    );
  }
}
