export interface SupportCategoryRole {
  id: string;
  supportCategoryId: string;
  roleName: string;
}

export interface SupportCategory {
  id: string;
  name: string;
  description?: string;
  createdAt: string;
  roles?: SupportCategoryRole[];
}
