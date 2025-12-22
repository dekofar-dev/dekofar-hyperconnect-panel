import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';

@Component({
  // eslint-disable-next-line @angular-eslint/component-selector
  selector: '<body[root]>',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent implements OnInit, AfterViewInit, OnDestroy {

  /** Video listesi */
  videos: string[] = [
    './assets/media/videos/auth-1.mp4',
    './assets/media/videos/auth-2.mp4',
  ];

  /** Seçilen video */
  currentVideo!: string;

  @ViewChild('bgVideo') bgVideo!: ElementRef<HTMLVideoElement>;

  constructor() {}

  ngOnInit(): void {
    this.setRandomVideo();
  }

  ngAfterViewInit(): void {
    const video = this.bgVideo.nativeElement;

    video.src = this.currentVideo;
    video.muted = true;
    video.loop = true;
    video.autoplay = true;
    video.playsInline = true;
    video.preload = 'auto';

    video.play().catch(() => {
      // iOS fallback
      document.addEventListener(
        'touchstart',
        () => video.play(),
        { once: true }
      );
    });
  }

  ngOnDestroy(): void {}

  private setRandomVideo(): void {
    const index = Math.floor(Math.random() * this.videos.length);
    this.currentVideo = this.videos[index];
  }
}
