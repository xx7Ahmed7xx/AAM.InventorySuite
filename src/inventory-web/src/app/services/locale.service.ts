import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type SupportedLocale = 'en' | 'ar';

@Injectable({
  providedIn: 'root'
})
export class LocaleService {
  private currentLocaleSubject = new BehaviorSubject<SupportedLocale>('en');
  public currentLocale$: Observable<SupportedLocale> = this.currentLocaleSubject.asObservable();

  constructor() {
    // Load saved locale from localStorage or default to 'en'
    const savedLocale = localStorage.getItem('appLocale') as SupportedLocale;
    if (savedLocale && (savedLocale === 'en' || savedLocale === 'ar')) {
      this.setLocale(savedLocale);
    } else {
      this.setLocale('en');
    }
  }

  getCurrentLocale(): SupportedLocale {
    return this.currentLocaleSubject.value;
  }

  setLocale(locale: SupportedLocale): void {
    this.currentLocaleSubject.next(locale);
    localStorage.setItem('appLocale', locale);
    
    // Update document direction for RTL support
    if (locale === 'ar') {
      document.documentElement.setAttribute('dir', 'rtl');
      document.documentElement.setAttribute('lang', 'ar');
    } else {
      document.documentElement.setAttribute('dir', 'ltr');
      document.documentElement.setAttribute('lang', 'en');
    }
  }
}

