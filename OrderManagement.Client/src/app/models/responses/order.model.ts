import { ILineItem } from './line-item.model';

export interface IOrder {
  id: string;
  orderNumber: number;
  userId: string;
  status: string;
  total: number;
  lineItems: ILineItem[];
}
