export interface ProductVariantSummary {
  productTitle: string;
  productImageUrl?: string;
  totalQuantity: number;
  variants: {
    variantTitle?: string;
    quantity: number;
    imageUrl?: string;
  }[];
}
