import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface MediaPreviewRequest {
  urls: string[];
}

export interface MediaDownloadRequest {
  imageIds?: string[];
  videoIds?: string[];
}

@Injectable({ providedIn: 'root' })
export class MediaDownloaderService {
  // 🔴 DEĞİŞTİ
  // Eskisi: `${environment.apiUrl}/media`
  private baseUrl = `${environment.apiUrl}/media-downloader`;
  // environment.apiUrl = 'http://localhost:5036/api'

  constructor(private http: HttpClient) {}

  preview(request: MediaPreviewRequest): Observable<any[]> {
    return this.http.post<any[]>(`${this.baseUrl}/preview`, request);
  }

  downloadImages(request: MediaDownloadRequest): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/download-images`, request, {
      responseType: 'blob'
    });
  }

  downloadVideos(request: MediaDownloadRequest): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/download-videos`, request, {
      responseType: 'blob'
    });
  }
}
