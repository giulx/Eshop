import { MoneyModel } from '../../../shared/models/money.model';

export interface CartItemModel {
  productId: number;
  name: string;
  quantity: number;
  unitPriceSnapshot: MoneyModel;
  subtotal: MoneyModel;
}
