import { OrderItemModel } from './order-item.model';

export interface OrderReadModel {
  id: number;
  customerId: number;
  creationDate: string;  // DateTime → string ISO
  status: number;        // enum OrderStatus lato backend (arriva come int)
  total: number;
  items: OrderItemModel[];
}
