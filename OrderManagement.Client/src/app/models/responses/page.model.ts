export interface IPage<TItem> {
  items: TItem[];
  pageNumber: number;
  pageSize: number;
  total: number;
}
