export interface MediaPreviewRequest {
  urls: string[];
}

export type MediaType = 'image' | 'video';

export interface MediaItemDto {
  id: string;
  pageUrl: string;
  mediaUrl: string;
  type: MediaType;
  originalFormat?: string | null;
  suggestedFormat?: string | null;
  width?: number | null;
  height?: number | null;
  sizeBytes?: number | null;
}

export interface MediaDownloadRequest {
  imageIds: string[];
  videoIds: string[];
}
