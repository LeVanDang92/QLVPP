import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiProblemDetails } from '../models/api-problem-details.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const apiError = error.error as ApiProblemDetails;

      const message =
        apiError?.detail ||
        apiError?.title ||
        'Có lỗi xảy ra. Vui lòng thử lại.';

      console.error('API Error:', {
        status: error.status,
        errorCode: apiError?.errorCode,
        message,
        traceId: apiError?.traceId
      });

      return throwError(() => error);
    })
  );
};
