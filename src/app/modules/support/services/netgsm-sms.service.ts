import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

/** Backend’in beklediği body */
export interface SmsInboxRequestBody {
  StartDate: string; // "YYYY-MM-DD HH:mm:ss"
  StopDate:  string; // "YYYY-MM-DD HH:mm:ss"
}

/** Backend’in döndürdüğü satır */
export interface SmsInboxResponse {
  orjinator: string; // gsmno
  message:   string; // msg
  date:      string; // tarih string
}

/** UI’de kullanılacak model */
export interface SmsMessageVm {
  fromNumber: string;
  message:    string;
  receivedAt: Date;
}

/** (Opsiyonel) Call logs body */
export interface CallLogRequestBody {
  StartDate: string;
  StopDate:  string;
  Direction?: 'IN' | 'OUT' | 'ALL';
}

@Injectable({ providedIn: 'root' })
export class NetgsmSmsService {
  private apiUrl = environment.apiUrl; // örn: http://localhost:5036

  constructor(private http: HttpClient) {}

  /** "YYYY-MM-DD HH:mm:ss" formatlayıcı */
  private formatDateTime(d: Date): string {
    const pad = (n: number) => (n < 10 ? '0' + n : '' + n);
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
  }

  /** Belirli tarih aralığı için SMS inbox */
  getSmsInboxByRange(from: Date, to: Date): Observable<SmsMessageVm[]> {
    const body: SmsInboxRequestBody = {
      StartDate: this.formatDateTime(from),
      StopDate:  this.formatDateTime(to),
    };

    return this.http.post<SmsInboxResponse[]>(
      `${this.apiUrl}/SmsInbox/list`,   // ✅ Güncel endpoint
      body
    ).pipe(
      map(list =>
        (list || [])
          .map(x => ({
            fromNumber: x.orjinator,
            message: x.message,
            receivedAt: new Date(x.date)
          }))
          .sort((a, b) => b.receivedAt.getTime() - a.receivedAt.getTime())
      )
    );
  }

  /** Son N günün inbox’ı (varsayılan 7 gün) */
  getSmsInboxLastDays(days = 7): Observable<SmsMessageVm[]> {
    const end = new Date(); end.setHours(23, 59, 59, 999);
    const start = new Date(); start.setDate(end.getDate() - days); start.setHours(0, 0, 0, 0);
    return this.getSmsInboxByRange(start, end);
  }

  /** Call logs XML (opsiyonel) */
  getCallLogsXmlByRange(from: Date, to: Date, direction: CallLogRequestBody['Direction'] = 'ALL'): Observable<string> {
    const body: CallLogRequestBody = {
      StartDate: this.formatDateTime(from),
      StopDate:  this.formatDateTime(to),
      Direction: direction
    };
    return this.http.post(
      `${this.apiUrl}/NetGsm/call-logs`,  // bu hala NetGsmController’da
      body,
      { responseType: 'text' }
    );
  }

  // ---- Client-side yardımcılar ----

  /** Basit arama */
  filterLocal(messages: SmsMessageVm[], term: string): SmsMessageVm[] {
    const t = (term || '').toLowerCase().trim();
    if (!t) return messages;
    return messages.filter(m =>
      (m.fromNumber || '').toLowerCase().includes(t) ||
      (m.message || '').toLowerCase().includes(t)
    );
  }

  /** Sayfalama */
  paginateLocal<T>(arr: T[], page: number, pageSize: number): T[] {
    const start = (page - 1) * pageSize;
    return arr.slice(start, start + pageSize);
  }
}
