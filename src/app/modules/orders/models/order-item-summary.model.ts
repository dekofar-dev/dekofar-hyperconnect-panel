// src/app/modules/orders/models/order-item-summary.model.ts
export interface OrderItemSummary {
  productId: number;
  variantId?: number;
  title: string;
  variantTitle?: string;
  totalQuantity: number;
  imageUrl?: string;
}

export interface VariantRow {
  variantId?: number;
  variantTitle?: string;
  quantity: number;
  imageUrl?: string;
}

export interface ProductGroup {
  productId: number;
  title: string;
  imageUrl?: string;
  totalQuantity: number;
  variants: VariantRow[];
}
