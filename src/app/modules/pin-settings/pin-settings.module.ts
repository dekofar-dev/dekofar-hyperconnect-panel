import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PinSettingsRoutingModule } from './pin-settings-routing.module';
import { PinSettingsComponent } from './pin-settings.component';

@NgModule({
  declarations: [PinSettingsComponent],
  imports: [CommonModule, FormsModule, PinSettingsRoutingModule]
})
export class PinSettingsModule {}
