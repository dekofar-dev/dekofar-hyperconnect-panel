import { Component } from '@angular/core';
import { forkJoin } from 'rxjs';
import {
  MediaItemDto,
  MediaDownloadRequest
} from '../media-downloader.models';
import { MediaDownloaderService } from '../media-downloader.service';

@Component({
  selector: 'app-media-downloader',
  templateUrl: './media-downloader.component.html',
  styleUrls: ['./media-downloader.component.scss']
})
export class MediaDownloaderComponent {

  urlsText = '';
  isLoadingPreview = false;
  isDownloading = false;

  allItems: MediaItemDto[] = [];
  imageItems: MediaItemDto[] = [];
  videoItems: MediaItemDto[] = [];

  selectedImageIds = new Set<string>();
  selectedVideoIds = new Set<string>();

  errorMessage: string | null = null;

  constructor(private mediaService: MediaDownloaderService) {}

  onPreview(): void {
    this.errorMessage = null;

    const urls = this.urlsText
      .split('\n')
      .map(x => x.trim())
      .filter(x => x.length > 0);

    if (!urls.length) {
      this.errorMessage = 'Lütfen en az bir ürün linki girin.';
      return;
    }

    this.isLoadingPreview = true;

    this.mediaService.preview({ urls }).subscribe({
      next: (items) => {
        this.allItems = items ?? [];

        this.imageItems = this.allItems.filter(x => x.type === 'image');
        this.videoItems = this.allItems.filter(x => x.type === 'video');

        this.selectedImageIds.clear();
        this.selectedVideoIds.clear();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Önizleme alınırken bir hata oluştu.';
      },
      complete: () => {
        this.isLoadingPreview = false;
      }
    });
  }

  // --- Seçim işlemleri ---

  toggleImageSelection(id: string, checked: boolean): void {
    if (checked) {
      this.selectedImageIds.add(id);
    } else {
      this.selectedImageIds.delete(id);
    }
  }

  toggleVideoSelection(id: string, checked: boolean): void {
    if (checked) {
      this.selectedVideoIds.add(id);
    } else {
      this.selectedVideoIds.delete(id);
    }
  }

  selectAllImages(): void {
    this.selectedImageIds.clear();
    this.imageItems.forEach(x => this.selectedImageIds.add(x.id));
  }

  clearAllImages(): void {
    this.selectedImageIds.clear();
  }

  selectAllVideos(): void {
    this.selectedVideoIds.clear();
    this.videoItems.forEach(x => this.selectedVideoIds.add(x.id));
  }

  clearAllVideos(): void {
    this.selectedVideoIds.clear();
  }

  // --- Download işlemi ---

  onDownload(): void {
    this.errorMessage = null;

    const imageIds = Array.from(this.selectedImageIds);
    const videoIds = Array.from(this.selectedVideoIds);

    if (!imageIds.length && !videoIds.length) {
      this.errorMessage = 'İndirmek için en az bir medya seçmelisiniz.';
      return;
    }

    const body: MediaDownloadRequest = {
      imageIds,
      videoIds
    };

    this.isDownloading = true;

    const tasks: any = {};

    if (imageIds.length) {
      tasks.images = this.mediaService.downloadImages(body);
    }

    if (videoIds.length) {
      tasks.videos = this.mediaService.downloadVideos(body);
    }

    forkJoin(tasks).subscribe({
      next: (result: any) => {
        if (result.images) {
          this.saveBlob(result.images, 'images.zip');
        }
        if (result.videos) {
          this.saveBlob(result.videos, 'videos.zip');
        }
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Dosyalar indirilirken bir hata oluştu.';
      },
      complete: () => {
        this.isDownloading = false;
      }
    });
  }

  private saveBlob(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    window.URL.revokeObjectURL(url);
  }

  formatSize(sizeBytes?: number | null): string {
    if (!sizeBytes || sizeBytes <= 0) {
      return '-';
    }

    const kb = sizeBytes / 1024;
    if (kb < 1024) {
      return `${kb.toFixed(1)} KB`;
    }

    const mb = kb / 1024;
    return `${mb.toFixed(1)} MB`;
  }

  get hasPreview(): boolean {
    return this.allItems.length > 0;
  }
}
