import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home';
import { BookingComponent } from './pages/booking/booking';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'booking', component: BookingComponent },
  { path: '**', redirectTo: '' },
];
