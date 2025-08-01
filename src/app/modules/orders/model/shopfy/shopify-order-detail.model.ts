import { NoteAttributeDto } from "./NoteAttributeDto";

export interface ShopifyOrderDetailDto {

  orderId: number;
  orderNumber: string; // ✅ düzeltildi
  createdAt: string;
  totalPrice: string;
  currency: string;
  financialStatus?: string;
  fulfillmentStatus?: string;
  note?: string;
  tags?: string;
  noteAttributes?: NoteAttributeDto[];

  customer: {
    id: number;
    firstName?: string;
    lastName?: string;
    phone?: string;
    email?: string;
    ordersCount?: number;
  };

  billingAddress: {
    firstName?: string;
    lastName?: string;
    address1?: string;
    address2?: string;
    city?: string;
    province?: string;
    country?: string;
    zip?: string;
    phone?: string;
  };

  lineItems: {
    title: string;
    variantTitle?: string;
    quantity: number;
    imageUrl?: string;
  }[];
}
