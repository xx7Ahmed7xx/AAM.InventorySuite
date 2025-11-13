import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule, registerLocaleData } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import localeEn from '@angular/common/locales/en';
import localeAr from '@angular/common/locales/ar';
import { AuthInterceptor } from './interceptors/auth.interceptor';

// Register locale data
registerLocaleData(localeEn);
registerLocaleData(localeAr);

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ProductsComponent } from './components/products/products.component';
import { CategoriesComponent } from './components/categories/categories.component';
import { StockMovementsComponent } from './components/stock-movements/stock-movements.component';
import { ReportsComponent } from './components/reports/reports.component';
import { LoginComponent } from './components/login/login.component';
import { UsersComponent } from './components/users/users.component';
import { LocalDatePipe } from './pipes/local-date.pipe';

@NgModule({
  declarations: [
    App,
    ProductsComponent,
    CategoriesComponent,
    StockMovementsComponent,
    ReportsComponent,
    LoginComponent,
    UsersComponent,
    LocalDatePipe
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [App]
})
export class AppModule { }
