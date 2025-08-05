export interface OrderModel {
  /** Sipariş ID (Shopify veya Manuel) */
  id: number | string;

  /** Sipariş numarası (manuel için elle atanır, Shopify için order_number) */
  orderNumber?: string;
  order_number?: string; // sadece Shopify için, gerekirse fallback

  /** Sipariş kaynağı */
  source: 'Shopify' | 'Manual';

  /** Sipariş oluşturulma zamanı */
  createdAt: string;

  /** Sipariş durumu */
  status?: string;

  /** Müşteri adı (manuelde tek string, Shopify’da birleştirilmiş) */
  customerName?: string;
  customerSurname?: string;

  /** İletişim bilgileri */
  phone?: string;
  email?: string;

  /** Adres bilgileri */
  address?: string;
  city?: string;
  district?: string;

  /** Ödeme türü (manuel için opsiyonel) */
  paymentType?: string;

  /** Sipariş notu */
  orderNote?: string;

  /** İndirim bilgileri (manuel siparişlerde opsiyonel) */
  discountId?: string;
  discountName?: string;
  discountType?: string;
  discountValue?: number;

  /** Toplam tutar */
  totalAmount: number;
  fulfillment_status?: 'fulfilled' | 'unfulfilled' | 'cancelled' | string;

  /** Kullanılan para birimi */
  currency: string;

  /** Ek bonus bilgisi (manuel) */
  bonusAmount?: number;

  /** Siparişi oluşturan kişinin avatar URL’si (manuel) */
  createdByAvatarUrl?: string;

  /** Sipariş içindeki ürünler */
  items: OrderItemModel[];
}

export interface OrderItemModel {
  /** Ürün ID (Shopify & Manuel) */
  productId: string;

  /** Varyant ID (isteğe bağlı) */
  variantId?: string;

  /** Ürün adı */
  productName: string;

  /** Varyant adı */
  variantName?: string;

  /** Adet */
  quantity: number;

  /** Tekil fiyat */
  price: number;
}
