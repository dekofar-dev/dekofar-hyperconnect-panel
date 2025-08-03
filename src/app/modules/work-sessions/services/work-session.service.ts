import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { WorkSession } from '../models/work-session.model';

const API_URL = `${environment.apiUrl}/WorkSessions`;

@Injectable({
  providedIn: 'root'
})
export class WorkSessionService {
  constructor(private http: HttpClient) {}

  startSession(): Observable<WorkSession> {
    return this.http.post<WorkSession>(`${API_URL}/start`, {});
  }

  endSession(): Observable<WorkSession | null> {
    return this.http.post<WorkSession | null>(`${API_URL}/end`, {});
  }

  getMySessions(): Observable<WorkSession[]> {
    return this.http.get<WorkSession[]>(`${API_URL}/my`);
  }

  getReport(filters: { userId?: string; startDate?: string; endDate?: string } = {}): Observable<WorkSession[]> {
    let params = new HttpParams();
    if (filters.userId) params = params.set('userId', filters.userId);
    if (filters.startDate) params = params.set('startDate', filters.startDate);
    if (filters.endDate) params = params.set('endDate', filters.endDate);
    return this.http.get<WorkSession[]>(`${API_URL}/report`, { params });
  }
}
