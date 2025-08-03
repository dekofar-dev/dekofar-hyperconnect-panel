import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorkSessionsListComponent } from './components/work-sessions-list/work-sessions-list.component';

/** Çalışma oturumları modülünün yönlendirme ayarları */
const routes: Routes = [
  // Listeleme sayfası
  { path: '', component: WorkSessionsListComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class WorkSessionsRoutingModule {}
