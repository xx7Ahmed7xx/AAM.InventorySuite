import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { LocaleService, SupportedLocale } from './services/locale.service';
import { TranslationService } from './services/translation.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App implements OnInit {
  currentLocale: SupportedLocale = 'en';

  constructor(
    public authService: AuthService,
    private router: Router,
    private localeService: LocaleService,
    public translate: TranslationService
  ) {
    // Ensure locale is initialized before components use it
    const savedLocale = localStorage.getItem('appLocale') as SupportedLocale;
    if (savedLocale && (savedLocale === 'en' || savedLocale === 'ar')) {
      this.currentLocale = savedLocale;
      this.localeService.setLocale(savedLocale);
    } else {
      this.currentLocale = 'en';
      this.localeService.setLocale('en');
    }
  }

  ngOnInit(): void {
    // Redirect to login if not authenticated
    if (!this.authService.isAuthenticated() && !this.router.url.includes('/login')) {
      this.router.navigate(['/login']);
    }
  }

  logout(): void {
    const confirmMessage = this.translate.translate('logout.confirm');
    if (confirm(confirmMessage)) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }

  changeLanguage(): void {
    this.localeService.setLocale(this.currentLocale);
  }
}
