export interface OrderSummary {
  id: number;
  customerName: string;
  customerId?: number;
  total: number;
  date: string;
  source: 'shopify' | 'manual';
}
