// src/app/core/models/api-problem-details.model.ts

export interface ApiProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;

  errorCode?: string;
  traceId?: string;

  errors?: Record<string, string[]>;
}
