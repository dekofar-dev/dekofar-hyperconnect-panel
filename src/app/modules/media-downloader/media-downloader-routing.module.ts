import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MediaDownloaderComponent } from './media-downloader/media-downloader.component';

const routes: Routes = [
  {
    path: '',
    component: MediaDownloaderComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MediaDownloaderRoutingModule {}
