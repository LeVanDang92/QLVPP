import { Component, input } from '@angular/core';

@Component({
  selector: 'app-form-error',
  standalone: true,
  template: `
    @if (message()) {
      <div class="error-tooltip-box">
        <span class="error-icon">⚠</span>
        <span class="error-text">{{ message() }}</span>
        <div class="error-arrow"></div>
      </div>
    }
  `,
  styles: [`
    :host {
      position: absolute;
      z-index: 1000;

      /* CĂN CHỈNH THÔNG MINH: Đẩy tooltip lên trên, neo theo lề phải của ô input lỗi */
      bottom: 100%;
      right: 12px;
      margin-bottom: 4px;

      /* Ngăn chặn tooltip co giật hoặc vỡ chữ */
      pointer-events: none;
      animation: tooltip-fade 0.2s cubic-bezier(0.16, 1, 0.3, 1);
    }

    .error-tooltip-box {
      display: flex;
      align-items: center;
      gap: 5px;
      background-color: #e02424;
      color: #ffffff;
      padding: 4px 10px;
      border-radius: 4px;
      font-size: 11px;
      font-weight: 500;
      white-space: nowrap;
      box-shadow: 0 4px 10px rgba(224, 36, 36, 0.2);
    }

    .error-arrow {
      position: absolute;
      bottom: -3px;
      right: 15px;
      width: 6px;
      height: 6px;
      background-color: #e02424;
      transform: rotate(45deg);
    }

    @keyframes tooltip-fade {
      from { opacity: 0; transform: translateY(4px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class FormErrorComponent {
  message = input<string | null>(null);
}
