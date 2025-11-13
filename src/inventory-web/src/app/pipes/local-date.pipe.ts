import { Pipe, PipeTransform, OnDestroy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { LocaleService } from '../services/locale.service';
import { Subscription } from 'rxjs';

/**
 * Pipe that converts UTC date strings to local time and formats them
 * according to the current locale
 */
@Pipe({
  name: 'localDate',
  standalone: false,
  pure: false // Make it impure so it updates when locale changes
})
export class LocalDatePipe implements PipeTransform, OnDestroy {
  private localeSubscription: Subscription;
  private currentLocale: string;

  constructor(private localeService: LocaleService) {
    // Initialize with current locale
    const locale = this.localeService.getCurrentLocale();
    this.currentLocale = locale === 'ar' ? 'ar-SA' : 'en-US';
    
    // Subscribe to locale changes
    this.localeSubscription = this.localeService.currentLocale$.subscribe(loc => {
      this.currentLocale = loc === 'ar' ? 'ar-SA' : 'en-US';
    });
  }

  ngOnDestroy(): void {
    if (this.localeSubscription) {
      this.localeSubscription.unsubscribe();
    }
  }

  transform(value: string | null | undefined, format: string = 'short'): string | null {
    if (!value) {
      return null;
    }

    // Get current locale fresh each time (in case it changed)
    const locale = this.localeService.getCurrentLocale();
    const localeString = locale === 'ar' ? 'ar-SA' : 'en-US';

    // Parse the UTC date string from backend
    // Backend sends dates in UTC format (ISO 8601)
    // We need to ensure dates are treated as UTC and converted to local time
    let date: Date;
    
    // Normalize the date string - replace space with T if needed
    let normalizedValue = value.replace(' ', 'T');
    
    // Check if the string already has timezone indicator
    const hasTimezone = normalizedValue.includes('Z') || /[+-]\d{2}:\d{2}$/.test(normalizedValue);
    
    if (hasTimezone) {
      // If it has timezone info, parse it directly
      // JavaScript Date constructor automatically converts UTC to local time
      date = new Date(normalizedValue);
    } else {
      // If no timezone indicator, assume it's UTC and append 'Z'
      // This ensures the date is parsed as UTC and then converted to local time
      // Backend sends dates like "2024-01-01T12:00:00" which JS interprets as local time
      // By adding 'Z', we tell JS it's UTC, and it will convert to local time
      if (!normalizedValue.endsWith('Z')) {
        normalizedValue = normalizedValue + 'Z';
      }
      date = new Date(normalizedValue);
    }

    if (isNaN(date.getTime())) {
      console.warn('Invalid date value:', value);
      return null;
    }

    // The date is now in local time (JavaScript Date always stores in local time)
    // Format it according to the current locale
    try {
      const datePipe = new DatePipe(localeString);
      return datePipe.transform(date, format);
    } catch (error) {
      // Fallback to default locale if there's an error
      console.warn('Error formatting date with locale', localeString, error);
      const datePipe = new DatePipe('en-US');
      return datePipe.transform(date, format);
    }
  }
}

