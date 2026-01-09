import { OrderReadModel } from './order-read.model';
import { UnorderableRowModel } from './unorderable-row.model';

export interface ConfirmOrderResultModel {
  success: boolean;
  errorCode?: string | null;
  order?: OrderReadModel | null;
  unorderedRows?: UnorderableRowModel[] | null;
}
