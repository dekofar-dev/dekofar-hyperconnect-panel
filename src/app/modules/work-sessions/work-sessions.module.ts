import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkSessionsRoutingModule } from './work-sessions-routing.module';
import { WorkSessionsListComponent } from './components/work-sessions-list/work-sessions-list.component';

/** Çalışma oturumları için modül */
@NgModule({
  declarations: [WorkSessionsListComponent],
  imports: [CommonModule, WorkSessionsRoutingModule],
})
export class WorkSessionsModule {}
