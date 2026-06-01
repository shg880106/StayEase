import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review, CreateReviewRequest } from '../models/review.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly apiUrl = `${environment.apiUrl}/review`;
  private readonly http = inject(HttpClient);

    
  getReviewDetails(reviewID: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.apiUrl}/review/${reviewID}`);
  }

  create(request: CreateReviewRequest): Observable<Review> {
    return this.http.post<Review>(this.apiUrl, request);
  }

  
}