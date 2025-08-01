export interface Order {
  id: number;
  order_number: string;
  created_at: string;
  total_price: string;
  currency: string;
  financial_status?: string;
  fulfillment_status?: string;
  customer?: Customer;
  billing_address?: BillingAddress;
  line_items: LineItem[];
  tags?: string;
  note?: string;
}

export interface Customer {
  id?: number;
  first_name?: string;
  last_name?: string;
  phone?: string;
  email?: string;
  orders_count?: number;
}

export interface BillingAddress {
  first_name?: string;
  last_name?: string;
  address1?: string;
  address2?: string;
  city?: string;
  province?: string;
  country?: string;
  zip?: string;
  phone?: string;
}

export interface LineItem {
  product_id: number;
  title?: string;
  variant_title?: string;
  quantity: number;
  image_url?: string; // Shopify'dan ürün görseli GraphQL ile alınan alan
}

export interface PagedResult<T> {
  items: T[];
  nextPageInfo?: string;
}
