import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LockScreenComponent } from './lock-screen.component';

@NgModule({
  declarations: [LockScreenComponent],
  imports: [CommonModule, FormsModule],
  exports: [LockScreenComponent]
})
export class LockScreenModule {}
