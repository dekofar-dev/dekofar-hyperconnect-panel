export interface OrderTagSummary {
  /** Shopify etiketi (örn: DHL, sms_sent, shipped) */
  tag: string;

  /** Bu etikete sahip AÇIK sipariş sayısı */
  orderCount: number;
}