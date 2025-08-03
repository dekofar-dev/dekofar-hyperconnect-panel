import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SupportTicketCreateDto, SupportTicketDto } from '../models/support-ticket.model';
import { PagedResult } from '../models/paged-result.model';
import { SupportTicketQuery } from '../models/support-ticket-query.model';
import { SupportTicketReplyDto } from '../models/support-ticket-reply.model';

@Injectable({ providedIn: 'root' })
export class SupportTicketService {
  private baseUrl = `${environment.apiUrl}/support-tickets`;

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

  // 🟢 Belirli destek talebini getir
  getById(id: string): Observable<SupportTicketDto> {
    return this.http.get<SupportTicketDto>(`${this.baseUrl}/${id}`);
  }

  // 🟢 Yeni destek talebi oluştur (FormData ile)
  create(data: SupportTicketCreateDto, files: File[] = []): Observable<SupportTicketDto> {
    const formData = new FormData();
    Object.entries(data).forEach(([key, value]) => {
      if (value !== undefined && value !== null) {
        formData.append(key, value.toString());
      }
    });
    files.forEach(file => {
      formData.append('attachments', file);
    });
    return this.http.post<SupportTicketDto>(this.baseUrl, formData);
  }

  // 🟢 Talebi kullanıcıya ata
  assignUser(ticketId: string, userId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/assign`, {
      ticketId,
      assignedToUserId: userId
    });
  }

  // 🟢 Talep durumunu güncelle
  updateStatus(ticketId: string, status: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${ticketId}/status`, {
      ticketId,
      status
    });
  }

  // 🟢 Talebi çözüldü olarak işaretle
  markAsResolved(ticketId: string): Observable<void> {
    return this.updateStatus(ticketId, 3);
  }

  // 🟢 Talebe yanıt gönder
  reply(ticketId: string, message: string, files: File[] = []): Observable<SupportTicketReplyDto> {
    const formData = new FormData();
    formData.append('ticketId', ticketId);
    formData.append('message', message);
    files.forEach(file => formData.append('attachments', file));
    return this.http.post<SupportTicketReplyDto>(`${this.baseUrl}/${ticketId}/reply`, formData);
  }

  // 🟢 Kullanıcının kendi destek taleplerini getir
  getMyTickets(): Observable<SupportTicketDto[]> {
    return this.http.get<SupportTicketDto[]>(`${this.baseUrl}/my`);
  }

  // 🔄 Atanabilir kullanıcıları getir (detay ekranı için)
  getAssignableUsers(): Observable<{ id: string; fullName: string; email: string; role: string }[]> {
    return this.http.get<{ id: string; fullName: string; email: string; role: string }[]>(
      `${environment.apiUrl}/users/assignable`
    );
  }

  markAsRead(ticketId: string): Observable<void> {
  return this.http.post<void>(`${this.baseUrl}/${ticketId}/read`, {});
}

update(ticketId: string, data: any): Observable<SupportTicketDto> {
  return this.http.put<SupportTicketDto>(`${this.baseUrl}/${ticketId}`, data);
}

addNote(ticketId: string, message: string): Observable<void> {
  return this.http.post<void>(`${this.baseUrl}/${ticketId}/note`, { message });
}

}
