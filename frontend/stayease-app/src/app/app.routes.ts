import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { BookingComponent } from './pages/booking/booking';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { MyPropertiesComponent } from './pages/my-properties/my-properties';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings';
import { ReviewComponent } from './pages/review/review';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'booking', component: BookingComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'my-properties', component: MyPropertiesComponent },
  { path: 'my-bookings', component: MyBookingsComponent },
  { path: 'review', component: ReviewComponent },
  { path: '**', redirectTo: '' },
];
