import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MediaDownloaderRoutingModule } from './media-downloader-routing.module';
import { MediaDownloaderComponent } from './media-downloader/media-downloader.component';

@NgModule({
  declarations: [MediaDownloaderComponent],
  imports: [
    CommonModule,
    FormsModule,
    MediaDownloaderRoutingModule
  ]
})
export class MediaDownloaderModule {}
