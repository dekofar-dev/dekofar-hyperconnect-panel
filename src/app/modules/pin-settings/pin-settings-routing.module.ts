import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PinSettingsComponent } from './pin-settings.component';

const routes: Routes = [
  { path: '', component: PinSettingsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PinSettingsRoutingModule {}
