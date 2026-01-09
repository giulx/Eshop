import { OrderRowModel } from './order-row.model';
import { UnorderableRowModel } from './unorderable-row.model';

export interface OrderPreviewModel {
  validRows: OrderRowModel[];
  discardedRows: UnorderableRowModel[];
  total: number;
  token?: string | null;
}
