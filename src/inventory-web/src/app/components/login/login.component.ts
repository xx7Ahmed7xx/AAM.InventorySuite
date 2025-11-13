import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, LoginDto } from '../../services/auth.service';
import { LocaleService, SupportedLocale } from '../../services/locale.service';
import { TranslationService } from '../../services/translation.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  credentials: LoginDto = {
    username: '',
    password: ''
  };
  error: string = '';
  loading: boolean = false;
  currentLocale: SupportedLocale = 'en';

  constructor(
    private authService: AuthService,
    private router: Router,
    private localeService: LocaleService,
    public translate: TranslationService
  ) {
    // If already logged in, redirect
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/products']);
    }
    
    // Load saved locale preference
    const savedLocale = localStorage.getItem('appLocale') as SupportedLocale;
    if (savedLocale && (savedLocale === 'en' || savedLocale === 'ar')) {
      this.currentLocale = savedLocale;
      this.localeService.setLocale(savedLocale);
    }
  }

  changeLanguage(): void {
    this.localeService.setLocale(this.currentLocale);
  }

  login(): void {
    if (!this.credentials.username || !this.credentials.password) {
      this.error = this.translate.translate('login.pleaseEnter');
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.login(this.credentials).subscribe({
      next: () => {
        this.router.navigate(['/products']);
      },
      error: (error) => {
        this.error = error.error?.message || this.translate.translate('login.invalid');
        this.loading = false;
      }
    });
  }
}

