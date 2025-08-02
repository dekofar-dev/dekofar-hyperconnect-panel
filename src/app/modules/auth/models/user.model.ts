// src/app/modules/auth/models/user.model.ts

export interface UserModel {
  id: string;
  email: string;
  fullName: string;
  role: string;

  // Metronic layout için gerekli alanlar 👇
  pic?: string;
  firstname?: string;
  lastname?: string;
  phone?: string;
  address?: string;
}
