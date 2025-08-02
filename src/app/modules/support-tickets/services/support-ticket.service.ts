import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SupportTicketCreateDto, SupportTicketDto } from '../models/support-ticket.model';
import { PagedResult } from '../models/paged-result.model';
import { SupportTicketQuery } from '../models/support-ticket-query.model';

@Injectable({
  providedIn: 'root'
})
export class SupportTicketService {
  private baseUrl = `${environment.apiUrl}/SupportTickets`;

  constructor(private http: HttpClient) {}

  // 🟢 Destek taleplerini filtre ve sayfalama ile getir
  list(query: SupportTicketQuery): Observable<PagedResult<SupportTicketDto>> {
    let params = new HttpParams();
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value as any);
      }
    });
    return this.http.get<PagedResult<SupportTicketDto>>(this.baseUrl, { params });
  }

  // 🟢 Belirli talebi getir
  getById(id: number): Observable<SupportTicketDto> {
    return this.http.get<SupportTicketDto>(`${this.baseUrl}/${id}`);
  }

  // 🟢 Yeni destek talebi oluştur
  create(data: SupportTicketCreateDto, files: File[] = []): Observable<SupportTicketDto> {
    const formData = new FormData();

    // Form alanlarını forma ekle
    Object.entries(data).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        formData.append(key, value.toString());
      }
    });

    // Dosyaları da ekle
    files.forEach(file => {
      formData.append('attachments', file);
    });

    return this.http.post<SupportTicketDto>(this.baseUrl, formData);
  }

  // 🟢 Talebe kullanıcı ata
  assignUser(ticketId: number, userId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/assign`, {
      assignedToUserId: userId
    });
  }

  // 🟢 Talep durumunu güncelle
  updateStatus(ticketId: number, status: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/status`, {
      status
    });
  }

  // 🟢 Talebi çözüldü olarak işaretle (status=3)
  markAsResolved(ticketId: number): Observable<void> {
    return this.updateStatus(ticketId, 3);
  }

  // 🟢 Talebi okundu olarak işaretle
  markAsRead(ticketId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/read`, {});
  }

  // 🟢 Talebe not ekle
  addNote(ticketId: number, message: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/note`, {
      message
    });
  }

  // 🟢 Talep güncelle (durum, öncelik, kategori vb.)
  update(ticketId: number, data: any): Observable<SupportTicketDto> {
    return this.http.put<SupportTicketDto>(`${this.baseUrl}/${ticketId}`, data);
  }

  // 🔄 Atanabilir kullanıcıları getir (detay ekranı için opsiyonel)
  getAssignableUsers(): Observable<{ id: string; fullName: string; email: string; role: string }[]> {
    return this.http.get<{ id: string; fullName: string; email: string; role: string }[]>(
      `${environment.apiUrl}/Users/assignable`
    );
  }
}
