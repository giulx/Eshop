import { CartItemModel } from './cart-item.model';
import { MoneyModel } from '../../../shared/models/money.model';

export interface CartModel {
  id: number;
  customerId: number;
  items: CartItemModel[];
  total: MoneyModel;
}
