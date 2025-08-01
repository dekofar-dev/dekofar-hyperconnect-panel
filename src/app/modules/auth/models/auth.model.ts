// src/app/modules/auth/models/auth.model.ts

import { UserModel } from './user.model';

export interface AuthModel {
  token: string;
  user: UserModel; // 👈 Eksik olan alan burasıydı
}
