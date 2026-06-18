import { Injectable, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AppConstants } from '../common/constant';

export interface AppLanguage {
  code: string;
  label: string;
  shortLabel: string;
}

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  private readonly translate = inject(TranslateService);
  private readonly storageKey = 'app_language';

  readonly languages: AppLanguage[] = [
    { code: 'vi', label: 'Tiếng Việt', shortLabel: 'VI' },
    { code: 'en', label: 'English', shortLabel: 'EN' },
    { code: 'ko', label: '한국어', shortLabel: 'KO' },
  ];

  readonly currentLanguage = signal<string>(AppConstants.APP_DEFAULT_LANGUAGE);

  initLanguage(): void {
    this.translate.addLangs(this.languages.map(x => x.code));

    // version mới dùng setFallbackLang thay cho setDefaultLang
    this.translate.setFallbackLang(AppConstants.APP_DEFAULT_LANGUAGE);

    const savedLanguage = localStorage.getItem(this.storageKey);

    const language =
      this.isSupportedLanguage(savedLanguage)
        ? savedLanguage!
        :  AppConstants.APP_DEFAULT_LANGUAGE;

    this.changeLanguage(language);
  }

  changeLanguage(languageCode: string): void {
    const language = this.isSupportedLanguage(languageCode)
      ? languageCode
      : AppConstants.APP_DEFAULT_LANGUAGE;

    this.translate.use(language);
    this.currentLanguage.set(language);
    localStorage.setItem(this.storageKey, language);
    document.documentElement.lang = language;
  }

  private isSupportedLanguage(language?: string | null): boolean {
    return !!language && this.languages.some(x => x.code === language);
  }
}
