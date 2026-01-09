export interface ProductModel {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  currency: string;
  availableQuantity: number;
}

export interface ProductCreateRequest {
  name: string;
  description?: string | null;
  price: number;
  currency?: string; // se omessa, il backend applica la valuta di default (es. "EUR")
  availableQuantity: number;
}

export interface ProductUpdateRequest {
  name?: string | null;
  description?: string | null;
  price?: number | null;
  currency?: string | null;
  availableQuantity?: number | null;
}
