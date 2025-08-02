// src/app/modules/auth/models/user.model.ts

export interface UserModel {
  id: string;
  email: string;
  fullName: string;
  role: string;

  // 👇 Opsiyonel Metronic alanları (backend'den geliyorsa sorun yok)
  pic?: string;
  firstname?: string;
  lastname?: string;
  phone?: string;
  address?: string;
}
