import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthRoutingModule } from './auth-routing.module';
import { AuthComponent } from './auth.component';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';

// Metronic için layout modülü (eğer kullanıyorsan)
import { LayoutModule } from '../../_metronic/layout';

@NgModule({
  declarations: [
    AuthComponent,
    LoginComponent,
    LogoutComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AuthRoutingModule,
    LayoutModule, // metronic layout desteği
  ],
})
export class AuthModule {}
