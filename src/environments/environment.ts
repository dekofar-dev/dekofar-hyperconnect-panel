// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  appVersion: 'v1.0.0',
  USERDATA_KEY: 'dekofar_user_data',
  isMockEnabled: false,

  // Lokal API adresi
    apiUrl: 'https://api.dekofar.com/api'
,

  // Siteye özgü bilgiler
  siteName: 'Herdemtaze',
  siteDomain: 'herdemtaze.com.tr',
  supportEmail: 'destek@herdemtaze.com.tr',

  // Tema bilgisi
  appThemeName: 'Metronic',
  appPurchaseUrl: 'https://1.envato.market/EA4JP',
  appPreviewUrl: 'https://preview.keenthemes.com/metronic8/angular/demo3/',
  appPreviewDocsUrl: 'https://preview.keenthemes.com/metronic8/angular/docs',
  appPreviewChangelogUrl: 'https://preview.keenthemes.com/metronic8/angular/docs/changelog',

  // Sadece demo3 kullanılacak
  appDemos: {
    demo3: {
      title: 'Demo 3',
      description: 'New Trend',
      published: true,
      thumbnail: './assets/media/demos/demo3.png'
    }
  }
};
