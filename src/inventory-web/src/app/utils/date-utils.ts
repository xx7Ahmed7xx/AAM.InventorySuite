/**
 * Utility functions for date/time handling
 */

/**
 * Converts a UTC date string to a local Date object
 * @param utcDateString ISO 8601 date string from API (assumed to be UTC)
 * @returns Date object in local timezone
 */
export function utcToLocalDate(utcDateString: string | null | undefined): Date | null {
  if (!utcDateString) {
    return null;
  }
  
  // If the string already includes timezone info, parse it directly
  // Otherwise, assume it's UTC and append 'Z'
  const dateString = utcDateString.includes('Z') || utcDateString.includes('+') || utcDateString.includes('-') && utcDateString.length > 19
    ? utcDateString
    : utcDateString + 'Z';
  
  return new Date(dateString);
}

/**
 * Formats a UTC date string for display in local time
 * @param utcDateString ISO 8601 date string from API
 * @param format Optional format string (default: 'short')
 * @returns Formatted date string in local time
 */
export function formatLocalDate(utcDateString: string | null | undefined, format: string = 'short'): string {
  const localDate = utcToLocalDate(utcDateString);
  if (!localDate) {
    return '';
  }
  
  // Use Intl.DateTimeFormat for formatting
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: format.includes('short') ? 'short' : 'numeric',
    day: 'numeric',
    hour: format.includes('time') || format.includes('short') ? 'numeric' : undefined,
    minute: format.includes('time') || format.includes('short') ? 'numeric' : undefined,
    second: format.includes('time') ? 'numeric' : undefined,
    hour12: true
  };
  
  return localDate.toLocaleString(undefined, options);
}

